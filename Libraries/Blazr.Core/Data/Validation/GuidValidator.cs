/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Core.Validation;

public class GuidValidator : Validator<Guid>
{

    /// <summary>
    /// Class Contructor
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fieldName"></param>
    /// <param name="model"></param>
    /// <param name="validationMessageStore"></param>
    /// <param name="message"></param>
    public GuidValidator(Guid value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message) 
        : base(value, fieldName, model, validationMessageStore, message) { }

    /// <summary>
    /// Check of the value is greater than test
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public GuidValidator NotEmpty(string? message = null)
    {
        if (!(this.Value != Guid.Empty))
        {
            Trip = true;
            LogMessage(message);
        }
        return this;
    }

}

public static class GuidValidatorExtensions
{
    /// <summary>
    /// String Validation Extension
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fieldName"></param>
    /// <param name="model"></param>
    /// <param name="validationMessageStore"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static GuidValidator Validation(this Guid value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message = null)
    {
        var validation = new GuidValidator(value, fieldName, model, validationMessageStore, message);
        return validation;
    }
}

