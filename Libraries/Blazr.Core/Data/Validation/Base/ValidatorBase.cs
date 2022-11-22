/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public abstract class ValidatorBase<T>
{
    protected readonly string fieldName = "Value";
    protected readonly T value;
    protected readonly string defaultMessage = "The value failed validation";
    protected readonly ValidationState validationState;
    protected readonly ValidationMessageCollection validationMessages = new();
    protected readonly ValidationMessageStore? validationMessageStore;
    protected readonly object? model;
    protected readonly Guid objectUid = Guid.Empty;
    protected readonly List<string> messages = new List<string>();

    public IEnumerable<string> Messages => this.messages;

    public ValidatorBase(T value, string? message = null)
    {
        this.value = value;
        this.validationState = new();
        this.defaultMessage = string.IsNullOrWhiteSpace(message)
            ? this.defaultMessage
            : message;

        this.validationMessageStore = null;
        this.model = null;
    }

    public ValidatorBase(T value, Guid objectUid, string fieldName, ValidationMessageCollection validationMessages, ValidationState validationState, string? message)
    {
        this.objectUid= objectUid;
        this.fieldName = fieldName;
        this.value = value;
        this.validationMessages = validationMessages;
        this.validationState = validationState;
        this.defaultMessage = string.IsNullOrWhiteSpace(message)
            ? this.defaultMessage
            : message;

        this.validationMessageStore = null;
        this.model = null;
    }

    public ValidatorBase(T value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
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


    public virtual ValidationResult Validate(string? field, string? message = null)
    {
        var needToLogMessages = string.IsNullOrEmpty(field) || this.fieldName.Equals(field);

        if (needToLogMessages && validationState.IsInvalid)
        {
            message ??= this.defaultMessage;

            // Check if we've logged specific messages.  If not add the default message
            if (this.messages.Count == 0) this.messages.Add(message);

            //If we have a ValidationMessageStore add the messages
            if (validationMessageStore is not null && model is not null)
                this.validationMessageStore?.Add(new FieldIdentifier(this.model, this.fieldName), this.Messages);

            this.validationMessages.Add(FieldReference.Create(objectUid, fieldName), this.Messages);
        }

        return new ValidationResult { IsValid = validationState.IsValid, ValidationMessages = this.validationMessages, ValidationNotRun = !needToLogMessages };
    }

    protected void LogMessage(string? message)
    {
        if (!string.IsNullOrWhiteSpace(message)) messages.Add(message);
    }

    protected void SetTripped()
        => validationState.Trip();

    protected void FailIfFalse(bool test, string? message)
    {
        if (!test)
        {
            validationState.Trip();
            LogMessage(message);
        }
    }

    protected void FailIfTrue(bool test, string? message)
    {
        if (test)
        {
            validationState.Trip();
            LogMessage(message);
        }
    }
}
