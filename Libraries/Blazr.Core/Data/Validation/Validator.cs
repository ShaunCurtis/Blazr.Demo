/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public abstract class Validator<T>
{
    protected readonly string fieldName;
    protected readonly T value;
    protected readonly string defaultMessage = "The value failed validation";
    protected readonly ValidationState validationState;
    protected readonly ValidationMessageStore? validationMessageStore;
    protected readonly object model;
    protected readonly List<string> messages = new List<string>();
    private bool _tripped;

    public IEnumerable<string> Messages => this.messages;

    public Validator(T value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
    {
        this.fieldName = fieldName;
        this.value = value;
        this.model = model;
        this.validationMessageStore = validationMessageStore;
        this.validationState = validationState;
        this.defaultMessage = string.IsNullOrWhiteSpace(message) 
            ? this.defaultMessage 
            : message;
    }

    public virtual void Validate(string? fieldname, string? message = null)
    {
        var needToLogMessages = string.IsNullOrEmpty(fieldname) || this.fieldName.Equals(fieldname);

        if (needToLogMessages && _tripped)
        {
            message ??= this.defaultMessage;

            // Check if we've logged specific messages.  If not add the default message
            if (this.messages.Count == 0) this.messages.Add(message);

            //set up a FieldIdentifier and add the message to the Edit Context ValidationMessageStore
            var fi = new FieldIdentifier(this.model, this.fieldName);

            this.validationMessageStore?.Add(fi, this.Messages);
        }
    }

    protected void LogMessage(string? message)
    {
        if (!string.IsNullOrWhiteSpace(message)) messages.Add(message);
    }

    protected void SetTripped()
        => _tripped = true;

    protected void FailIfFalse(bool test, string? message)
    {
        if (!test)
        {
            this.SetTripped();
            LogMessage(message);
        }
    }

    protected void FailIfTrue(bool test, string? message)
    {
        if (test)
        {
            this.SetTripped();
            LogMessage(message);
        }
    }
}

