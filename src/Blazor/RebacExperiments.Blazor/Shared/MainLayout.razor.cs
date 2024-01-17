﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RebacExperiments.Blazor.Shared
{
    public partial class MainLayout
    {
        private const string JAVASCRIPT_FILE = "./Shared/MainLayout.razor.js";
        private string? _version;
        private bool _mobile;
        private string? _prevUri;
        private bool _menuChecked = true;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public RenderFragment? Body { get; set; }

        private ErrorBoundary? _errorBoundary;

        protected override void OnInitialized()
        {
            _version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            _prevUri = NavigationManager.Uri;
            NavigationManager.LocationChanged += LocationChanged;
        }

        protected override void OnParametersSet()
        {
            _errorBoundary?.Recover();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", JAVASCRIPT_FILE);
                _mobile = await jsModule.InvokeAsync<bool>("isDevice");
                await jsModule.DisposeAsync();
            }
        }

        private void HandleException(Exception exception)
        {
            (var errorCode, var errorMessage) = ApplicationErrorTranslator.GetErrorMessage(exception);

            MessageService.ShowMessageBar(options =>
            {
                options.Section = App.MESSAGES_TOP;
                options.Intent = MessageIntent.Error;
                options.ClearAfterNavigation = false;
                options.Title = "Error";
                options.Body = errorMessage;
                options.Timestamp = DateTime.Now;
                options.Link = new ActionLink<Message>
                {
                    Text = Loc["Message_ShowHelp"], 
                    Href = $"Help/ErrorDetails/{errorCode}", // Open Help in a new window...
                };
            });
        }



        private void HandleChecked()
        {
            _menuChecked = !_menuChecked;
        }

        private void LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (!e.IsNavigationIntercepted && new Uri(_prevUri!).AbsolutePath != new Uri(e.Location).AbsolutePath)
            {
                _prevUri = e.Location;
                if (_mobile && _menuChecked == true)
                {
                    _menuChecked = false;
                    StateHasChanged();
                }
            }
        }
    }
}