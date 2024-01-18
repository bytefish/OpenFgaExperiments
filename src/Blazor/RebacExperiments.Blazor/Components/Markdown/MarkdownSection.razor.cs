// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using RebacExperiments.Blazor.Infrastructure;

namespace RebacExperiments.Blazor.Components
{
    /// <summary>
    /// This is based on https://www.fluentui-blazor.net/Lab/MarkdownSection.
    /// </summary>
    public partial class MarkdownSection : FluentComponentBase
    {
        private string? _content;
        private bool _raiseContentConverted;

        [Inject]
        private IStaticAssetService StaticAssetService { get; set; } = default!;

        /// <summary>
        /// Gets or sets asset to read the Markdown from.
        /// </summary>
        [Parameter]
        public required string FromAsset { get; set; }

        /// <summary>
        /// Raised, when the Content has been converted.
        /// </summary>
        [Parameter]
        public EventCallback OnContentConverted { get; set; }

        /// <summary>
        /// Sanitized HTML Content.
        /// </summary>
        public MarkupString HtmlContent { get; private set; }

        public string? InternalContent
        {
            get => _content;
            set
            {
                _content = value;

                HtmlContent = ConvertToMarkupString(_content);

                if (OnContentConverted.HasDelegate)
                {
                    OnContentConverted.InvokeAsync();
                }

                _raiseContentConverted = true;

                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            InternalContent = await StaticAssetService.GetAsync(FromAsset);

            if (_raiseContentConverted)
            {
                _raiseContentConverted = false;

                if (OnContentConverted.HasDelegate)
                {
                    await OnContentConverted.InvokeAsync();
                }
            }
        }

        private static MarkupString ConvertToMarkupString(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                // Convert markdown string to HTML
                string? html = Markdown.ToHtml(value, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

                // Return sanitized HTML as a MarkupString that Blazor can render
                return new MarkupString(html);
            }

            return new MarkupString();
        }
    }
}
