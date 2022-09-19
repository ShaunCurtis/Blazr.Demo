/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

/// <summary>
/// Component Class that adds Edit State and Validation State to a Blazor EditForm Control
/// Should be placed within thr EditForm ontrol
/// </summary>
public class ValidationFormState : ComponentBase, IDisposable
{

    /// <summary>
    /// EditContext - cascaded from EditForm
    /// </summary>
    [CascadingParameter] public EditContext? EditContext { get; set; }

    /// <summary>
    /// Property to set if thr control does Validation on every field change
    /// Default is true
    /// </summary>
    [Parameter] public bool DoValidationOnFieldChange { get; set; } = true;

    /// <summary>
    /// EventCallback for parent to link into for Validation State Change Events
    /// passes the current validation State
    /// </summary>
    [Parameter] public EventCallback<bool> ValidStateChanged { get; set; }

    /// <summary>
    /// Property to expose the Validation State of the Control
    /// </summary>
    public bool IsValid => !EditContext?.GetValidationMessages().Any() ?? true;

    private ValidationMessageStore? _validationMessageStore;
    private ValidationMessageStore validationMessageStore => _validationMessageStore!;
    private bool validating = false;
    private bool disposedValue;

    protected override Task OnInitializedAsync()
    {
        Debug.Assert(this.EditContext != null);

        if (this.EditContext != null)
        {
            // Get the Validation Message Store from the EditContext
            _validationMessageStore = new ValidationMessageStore(this.EditContext);
            // Wires up to the EditContext OnFieldChanged event
            if (this.DoValidationOnFieldChange)
                this.EditContext.OnFieldChanged += FieldChanged;
            // Wires up to the Editcontext OnValidationRequested event
            this.EditContext.OnValidationRequested += ValidationRequested;
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Event Handler for Editcontext.OnFieldChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FieldChanged(object? sender, FieldChangedEventArgs e)
        => this.Validate(e.FieldIdentifier.FieldName);

    /// <summary>
    /// Event Handler for EditContext.OnValidationRequested
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ValidationRequested(object? sender, ValidationRequestedEventArgs e)
        => this.Validate();

    /// <summary>
    /// Validation Method
    /// if validating is true then we have a validation queued/inprocess so don't need to queue another one
    /// </summary>
    /// <param name="fieldname"></param>
    private void Validate(string? fieldname = null)
    {
        // Checks to see if the Model implements IValidation
        var validator = this.EditContext!.Model as IValidation;
        if (validator != null || !this.validating)
        {
            this.validating = true;
            // Check if we are doing a field level or form level validation
            // Form level - clear all validation messages
            // Field level - clear any field specific validation messages
            if (string.IsNullOrEmpty(fieldname))
                this.validationMessageStore?.Clear();
            else
                validationMessageStore?.Clear(new FieldIdentifier(this.EditContext.Model, fieldname));
            // Run the IValidation interface Validate method
            validator?.Validate(validationMessageStore, fieldname, this.EditContext.Model);
            // Notify the EditContext that the Validation State has changed - 
            // This precipitates a OnValidationStateChanged event which the validation message controls are all plugged into
            this.EditContext.NotifyValidationStateChanged();
            // Invoke ValidationStateChanged
            this.ValidStateChanged.InvokeAsync(this.IsValid);
            this.validating = false;
        }
    }

    /// <summary>
    /// Method to clear the Validation and Edit State 
    /// </summary>
    public void Clear()
        => this.validationMessageStore.Clear();

    // IDisposable Implementation
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (this.EditContext != null)
                {
                    this.EditContext.OnFieldChanged -= this.FieldChanged;
                    this.EditContext.OnValidationRequested -= this.ValidationRequested;
                }
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
