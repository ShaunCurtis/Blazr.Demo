/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public abstract class Validator<T>
{
    /// <summary>
    /// True if the values passed Validation
    /// </summary>
    public bool IsValid => !Trip;

    /// <summary>
    /// Messages to display if validation fails
    /// </summary>
    public List<string> Messages { get; } = new List<string>();

    /// <summary>
    /// Tripwire for validation failure
    /// </summary>
    protected bool Trip { get; set; } = false;

    /// <summary>
    /// Class Contructor
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fieldName"></param>
    /// <param name="model"></param>
    /// <param name="validationMessageStore"></param>
    /// <param name="message"></param>
    public Validator(T value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message)
    {
        this.FieldName = fieldName;
        this.Value = value;
        this.Model = model;
        this.ValidationMessageStore = validationMessageStore;
        this.DefaultMessage = string.IsNullOrWhiteSpace(message) 
            ? this.DefaultMessage 
            : message;
    }

    /// <summary>
    /// Method to Log the Validation to the Validation Store and trip a tripwire
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public virtual bool Validate(ref bool tripwire, string? fieldname, string? message = null)
    {
        if (string.IsNullOrEmpty(fieldname) || this.FieldName.Equals(fieldname))
        {
            this.Validate(message);
            if (!this.IsValid)
                tripwire = true;
        }
        else this.Trip = false;
        return this.IsValid;
    }

    /// <summary>
    /// Name of the Field
    /// </summary>
    protected string FieldName { get; set; }

    /// <summary>
    /// Field Value
    /// </summary>
    protected T Value { get; set; }

    /// <summary>
    /// Default message to diplay if failed validation
    /// </summary>
    protected string DefaultMessage { get; set; } = "The value failed validation";

    /// <summary>
    /// Reference to the current Edit Context ValidationMessageStore
    /// </summary>
    protected ValidationMessageStore? ValidationMessageStore { get; set; }

    protected object Model { get; set; }


    /// <summary>
    /// Method to Log the Validation to the Validation Store
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected virtual bool Validate(string? message = null)
    {
        if (!this.IsValid)
        {
            message ??= this.DefaultMessage;
            // Check if we've logged specific messages.  If not add the default message
            if (this.Messages.Count == 0) Messages.Add(message);
            //set up a FieldIdentifier and add the message to the Edit Context ValidationMessageStore
            var fi = new FieldIdentifier(this.Model, this.FieldName);
            this.ValidationMessageStore?.Add(fi, this.Messages);
        }
        return this.IsValid;
    }

    /// <summary>
    /// Method to add a message to the log
    /// </summary>
    /// <param name="message"></param>
    protected void LogMessage(string? message)
    {
        if (!string.IsNullOrWhiteSpace(message)) Messages.Add(message);
    }
}

