// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RebacExperiments.Blazor.Infrastructure;
using RebacExperiments.Shared.ApiSdk;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// We need the CookieHandler to send the Authentication Cookie to the Server.
builder.Services.AddScoped<CookieHandler>();

// Add the Kiota Client.
builder.Services.AddScoped<IAuthenticationProvider, AnonymousAuthenticationProvider>();

builder.Services
    .AddHttpClient<IRequestAdapter, HttpClientRequestAdapter>(client => client.BaseAddress = new Uri("https://localhost:5000"))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped<ApiClient>();

// Localization
builder.Services.AddLocalization();

// Fluent UI
builder.Services.AddFluentUIComponents();

//When using icons and/or emoji replace the line above with the code below
//LibraryConfiguration config = new(ConfigurationGenerator.GetIconConfiguration(), ConfigurationGenerator.GetEmojiConfiguration());
//builder.Services.AddFluentUIComponents(config);

await builder.Build().RunAsync();
