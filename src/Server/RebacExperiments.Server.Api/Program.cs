// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.EntityFrameworkCore;
using OpenFga.Sdk.Client;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.OData;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Database;
using RebacExperiments.Server.OpenFga;
using Serilog;
using Serilog.Core;
using Serilog.Filters;
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
    .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"))
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

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITaskItemService, TaskItemService>();
    builder.Services.AddScoped<ITeamService, TeamService>();
    builder.Services.AddScoped<IOrganizationService, OrganizationService>();
    builder.Services.AddScoped<IAclService, AclService>();

    // Logging
    builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

    // Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
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

    builder.Services.AddDbContext<OpenFgaDbContext>(options =>
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

    // Add ODataExceptionHandler
    builder.Services.Configure<ODataExceptionHandlerOptions>(o =>
    {
        o.IncludeExceptionDetails = builder.Environment.IsDevelopment() || builder.Environment.IsStaging();
    });

    builder.Services.AddExceptionHandler<ODataExceptionHandler>();

    // Cookie Authentication
    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax; // We don't want to deal with CSRF Tokens

            options.Events.OnRedirectToLogin = (context) => throw new AuthenticationFailedException();
            options.Events.OnRedirectToAccessDenied = (context) => throw new AuthorizationFailedException();
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
        options.OnRejected = (ctx, cancellationToken) => throw new RateLimitException();

        options.AddPolicy(Policies.PerUserRatelimit, context =>
        {
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

    app.UseExceptionHandler(_ => { }); // // https://github.com/dotnet/aspnetcore/issues/51888

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
    app.UseRateLimiter();
    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "An unhandeled exception occured.");
}
finally
{
    // Wait 0.5 seconds before closing and flushing, to gather the last few logs.
    await Task.Delay(TimeSpan.FromMilliseconds(500));
    await Log.CloseAndFlushAsync();
}