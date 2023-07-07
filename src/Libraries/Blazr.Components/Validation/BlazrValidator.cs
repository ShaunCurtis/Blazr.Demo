using FluentValidation;
using FluentValidation.Results;

namespace Blazr.Components;

public sealed class BlazrValidator<TRecord, TValidator> : BlazrControlBase, IDisposable
    where TRecord : class
    where TValidator : class, IValidator<TRecord>, new()
{
    [CascadingParameter] private EditContext _editContext { get; set; } = default!;

    private ValidationMessageStore _validationMessageStore = default!;
    private TValidator _validator = default!;
    private TRecord _model = default!;

    protected override Task OnParametersSetAsync()
    {
        if(NotInitialized)
        {
            // validate the edit context is not null
            ArgumentNullException.ThrowIfNull(_editContext);

            // Get a validation store instance and validate is not null
            _validationMessageStore = new ValidationMessageStore(_editContext);
            ArgumentNullException.ThrowIfNull(_validationMessageStore);

            // Create an instance of the Validator and validate is not null
            _validator = Activator.CreateInstance<TValidator>();
            ArgumentNullException.ThrowIfNull(_validator);

            // Get a reference to the model, validate not null and cast it
            var model = _editContext.Model as TRecord;
            ArgumentNullException.ThrowIfNull(model);
            _model = model;

            // Set up the listeners on the edit context so we can run validation
            _editContext.OnValidationRequested += OnValidationRequested;
            _editContext.OnFieldChanged += OnFieldChanged;
        }

        return Task.CompletedTask;
    }

    // Method to handle a Validation Event - need to validate the whole model
    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        _validationMessageStore.Clear();

        var result = _validator.Validate(_model);
        this.LogErrors(result);

        _editContext.NotifyValidationStateChanged();
    }

    // Method to handle a field change Event - need to validate only the specific field
    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        _validationMessageStore.Clear(e.FieldIdentifier);

        var result = _validator.Validate(_model, options =>
        {
            options.IncludeProperties(e.FieldIdentifier.FieldName);
        });

        this.LogErrors(result);

        _editContext.NotifyValidationStateChanged();
    }

    private void LogErrors(ValidationResult result)
    {
        if (result.IsValid)
            return;

        foreach (var error in result.Errors)
        {
            var fi = new FieldIdentifier(error.CustomState ?? _model, error.PropertyName);
            _validationMessageStore.Add(fi, error.ErrorMessage);
        }
    }

    public void Dispose()
    {
        _editContext.OnValidationRequested -= OnValidationRequested;
        _editContext.OnFieldChanged -= OnFieldChanged;
    }
}
