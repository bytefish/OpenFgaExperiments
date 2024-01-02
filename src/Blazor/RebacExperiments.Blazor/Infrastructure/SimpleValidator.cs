using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Reflection;

namespace RebacExperiments.Blazor.Infrastructure
{
    public class SimpleValidator<TModel> : ComponentBase, IDisposable
    {
        private IDisposable? _subscriptions;
        private EditContext? _originalEditContext;

        [CascadingParameter] EditContext? CurrentEditContext { get; set; }

        [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

        [Parameter]
        public Func<TModel?, IServiceProvider, IEnumerable<ValidationError>> ValidationFunc { get; set; } = null!;

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(SimpleValidator<TModel>)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(DataAnnotationsValidator)} " +
                    $"inside an EditForm.");
            }

            _subscriptions = CurrentEditContext.EnableSimpleValidation(ServiceProvider, ValidationFunc);
            _originalEditContext = CurrentEditContext;
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (CurrentEditContext != _originalEditContext)
            {
                // While we could support this, there's no known use case presently. Since InputBase doesn't support it,
                // it's more understandable to have the same restriction.
                throw new InvalidOperationException($"{GetType()} does not support changing the " +
                    $"{nameof(EditContext)} dynamically.");
            }
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            _subscriptions?.Dispose();
            _subscriptions = null;

            Dispose(disposing: true);
        }
    }

    public record ValidationError
    {
        public required string PropertyName { get; set; }

        public required string ErrorMessage { get; set; }
    }

    public static class EditContextDataAnnotationsExtensions
    {
        /// <summary>
        /// Enables DataAnnotations validation support for the <see cref="EditContext"/>.
        /// </summary>
        /// <param name="editContext">The <see cref="EditContext"/>.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to be used in the <see cref="ValidationContext"/>.</param>
        /// <returns>A disposable object whose disposal will remove DataAnnotations validation support from the <see cref="EditContext"/>.</returns>
        public static IDisposable EnableSimpleValidation<TModel>(this EditContext editContext, IServiceProvider serviceProvider, Func<TModel?, IServiceProvider, IEnumerable<ValidationError>> validationFunc)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            return new SimpleValidationEventSubscriptions<TModel>(editContext, serviceProvider, validationFunc);
        }

        private sealed class SimpleValidationEventSubscriptions<TModel> : IDisposable
        {
            private readonly EditContext _editContext;
            private readonly IServiceProvider _serviceProvider;
            private readonly Func<TModel?, IServiceProvider, IEnumerable<ValidationError>> _validationFunc;
            private readonly ValidationMessageStore _messages;

            public SimpleValidationEventSubscriptions(EditContext editContext, IServiceProvider serviceProvider, Func<TModel?, IServiceProvider, IEnumerable<ValidationError>> validationFunc)
            {
                _editContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
                _serviceProvider = serviceProvider;
                _validationFunc = validationFunc;
                _messages = new ValidationMessageStore(_editContext);

                _editContext.OnFieldChanged += OnFieldChanged;
                _editContext.OnValidationRequested += OnValidationRequested;

            }

            [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Model types are expected to be defined in assemblies that do not get trimmed.")]
            private void OnFieldChanged(object? sender, FieldChangedEventArgs eventArgs)
            {
                _messages.Clear();

                var validationErrors = _validationFunc((TModel) _editContext.Model, _serviceProvider);

                foreach (var validationError in validationErrors)
                {
                    _messages.Add(_editContext.Field(validationError.PropertyName), validationError.ErrorMessage);
                }

                _editContext.NotifyValidationStateChanged();
            }

            [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Model types are expected to be defined in assemblies that do not get trimmed.")]
            private void OnValidationRequested(object? sender, ValidationRequestedEventArgs eventArgs)
            {
                _messages.Clear();

                var validationErrors = _validationFunc((TModel) _editContext.Model, _serviceProvider);

                foreach (var validationError in validationErrors)
                {
                    _messages.Add(_editContext.Field(validationError.PropertyName), validationError.ErrorMessage);
                }

                _editContext.NotifyValidationStateChanged();
            }

            public void Dispose()
            {
                _messages.Clear();
                _editContext.OnFieldChanged -= OnFieldChanged;
                _editContext.OnValidationRequested -= OnValidationRequested;
                _editContext.NotifyValidationStateChanged();

            }
        }
    }
}
