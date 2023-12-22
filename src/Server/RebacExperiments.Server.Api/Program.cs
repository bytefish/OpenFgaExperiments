// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OData;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Model;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.OData;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Database;
using RebacExperiments.Server.OpenFga;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Security.Claims;
using System.Threading.RateLimiting;

// We will log to %LocalAppData%/RebacExperiments to store the Logs, so it doesn't need to be configured 
// to a different path, when you run it on your machine.
string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RebacExperiments");

// We are writing with RollingFileAppender using a daily rotation, and we want to have the filename as 
// as "LogRebacExperiments-{Date}.log", the "{Date}" placeholder will be replaced by Serilog itself.
string logFilePath = Path.Combine(logDirectory, "LogRebacExperiments-.log");

// Configure the Serilog Logger. This Serilog Logger will be passed 
// to the Microsoft.Extensions.Logging LoggingBuilder using the 
// LoggingBuilder#AddSerilog(...) extension.
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    // Add services to the container
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddSingleton<IUserService, UserService>();

    builder.Services.AddSingleton<ITaskItemService, TaskItemService>();
    builder.Services.AddSingleton<ITeamService, TeamService>();
    builder.Services.AddSingleton<IOrganizationService, OrganizationService>();
    builder.Services.AddSingleton<IAclService, AclService>();

    // Logging
    builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

    // Database
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("ApplicationDatabase");

        if (connectionString == null)
        {
            throw new InvalidOperationException("No ConnectionString named 'ApplicationDatabase' was found");
        }

        options
            .EnableSensitiveDataLogging()
            .UseSqlServer(connectionString);
    });

    builder.Services.AddDbContextFactory<OpenFgaDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("OpenFgaDatabase");

        if (connectionString == null)
        {
            throw new InvalidOperationException("No ConnectionString named 'OpenFgaDatabase' was found");
        }

        options
            .EnableSensitiveDataLogging()
            .UseNpgsql(connectionString);
    });

    // OpenFGA
    builder.Services.AddSingleton<OpenFgaClient>(sp =>
    {
        var configuration = new ClientConfiguration
        {
            ApiUrl = builder.Configuration.GetValue<string>("OpenFGA:ApiUrl")!,
            StoreId = builder.Configuration.GetValue<string>("OpenFGA:StoreId")!,
            AuthorizationModelId = builder.Configuration.GetValue<string>("OpenFGA:AuthorizationModelId")!,
        };

        return new OpenFgaClient(configuration);
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>();

        if (allowedOrigins == null)
        {
            throw new InvalidOperationException("AllowedOrigins is missing in the appsettings.json");
        }

        options.AddPolicy("CorsPolicy", builder => builder
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
    });

    // Add Error Handler
    builder.Services.Configure<ApplicationErrorHandlerOptions>(o =>
    {
        o.IncludeExceptionDetails = builder.Environment.IsDevelopment() || builder.Environment.IsStaging();
    });

    builder.Services.AddSingleton<ApplicationErrorHandler>();

    // Cookie Authentication
    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax; // We don't want to deal with CSRF Tokens

            options.Events.OnRedirectToAccessDenied = (context) => throw new AuthenticationFailedException(); // Handle this in the middleware ...
            options.Events.OnRedirectToLogin = (context) => throw new AuthenticationFailedException(); // Handle this in the middleware ...
        });

    builder.Services
        .AddControllers()
        // Register OData Routes:
        .AddOData((options) =>
        {
            options.AddRouteComponents(routePrefix: "odata", model: ApplicationEdmModel.GetEdmModel(), configureServices: svcs =>
            {
                svcs.AddSingleton<ODataBatchHandler>(new DefaultODataBatchHandler());
            });

            // Do not enable $expand
            options.Select().OrderBy().Filter().SetMaxTop(250).Count();
        });

    builder.Services.AddSwaggerGen();

    // Add Policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(Policies.RequireUserRole, policy => policy.RequireRole(Roles.User));
        options.AddPolicy(Policies.RequireAdminRole, policy => policy.RequireRole(Roles.Administrator));
    });

    // Add the Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddPolicy(Policies.PerUserRatelimit, context =>
        {
            // We always have a user name
            var username = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return RateLimitPartition.GetTokenBucketLimiter(username, key =>
            {
                return new()
                {
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    AutoReplenishment = true,
                    TokenLimit = 100,
                    TokensPerPeriod = 100,
                    QueueLimit = 100,
                };
            });
        });
    });

    var app = builder.Build();

    // We want all Exceptions to return an ODataError in the Response. So all Exceptions should be handled and run through
    // this ExceptionHandler. This should only happen for things deep down in the ASP.NET Core stack, such as not resolving
    // routes.
    // 
    // Anything else should run through the Controllers and the Error Handlers are going to work there.
    //
    app.UseExceptionHandler(options =>
    {
        options.Run(async context =>
        {
            // Get the ExceptionHandlerFeature, so we get access to the original Exception
            var exceptionHandlerFeature = context.Features.GetRequiredFeature<IExceptionHandlerFeature>();
            
            // The ODataErrorHandler is required for adding an ODataError to all failed HTTP responses
            var odataErrorHandler = context.RequestServices.GetRequiredService<ApplicationErrorHandler>();

            // We can get the underlying Exception from the ExceptionHandlerFeature
            var exception = exceptionHandlerFeature.Error;

            // This isn't nice, because we probably shouldn't work with MVC types here. It would be better 
            // to rewrite the ApplicationErrorHandler to working with the HttpResponse.
            var actionContext = new ActionContext { HttpContext = context };

            var actionResult = odataErrorHandler.HandleException(context, exception);

            await actionResult
                .ExecuteResultAsync(actionContext)
                .ConfigureAwait(false);
        });
    });

    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("https://localhost:5000/odata/openapi.json", "TaskManagement Service");
        });
    }

    app.UseCors("CorsPolicy");

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
} 
catch(Exception exception)
{
    Log.Fatal(exception, "An unhandeled exception occured.");
}
finally
{
    // Wait 0.5 seconds before closing and flushing, to gather the last few logs.
    await Task.Delay(TimeSpan.FromMilliseconds(500));
    await Log.CloseAndFlushAsync();
}