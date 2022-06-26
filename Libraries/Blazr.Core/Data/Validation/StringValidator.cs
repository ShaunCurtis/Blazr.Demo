/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core.Validation;

public class StringValidator : Validator<string>
{

    /// <summary>
    /// Class Contructor
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fieldName"></param>
    /// <param name="model"></param>
    /// <param name="validationMessageStore"></param>
    /// <param name="message"></param>
    public StringValidator(string value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message)
        : base(value, fieldName, model, validationMessageStore, message) { }

    /// <summary>
    /// Check of the string is longer than test
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public StringValidator LongerThan(int test, string? message = null)
    {
        if (string.IsNullOrEmpty(this.Value) || !(this.Value.Length > test))
        {
            Trip = true;
            LogMessage(message);
        }
        return this;
    }

    /// <summary>
    /// Check if the string is shorter than
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public StringValidator ShorterThan(int test, string? message = null)
    {

        if (string.IsNullOrEmpty(this.Value) || !(this.Value.Length < test))
        {
            Trip = true;
            LogMessage(message);
        }
        return this;
    }

    /// <summary>
    /// Check if the string matches a regex pattern
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public StringValidator Matches(string pattern, string? message = null)
    {
        if (!string.IsNullOrWhiteSpace(this.Value))
        {
            var match = Regex.Match(this.Value, pattern);
            if (match.Success && match.Value.Equals(this.Value)) return this;
        }
        this.Trip = true;
        LogMessage(message);
        return this;
    }
}

public static class StringValidatorExtensions
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
    public static StringValidator Validation(this string value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message = null)
    {
            return new StringValidator(value, fieldName, model, validationMessageStore, message);
    }
}
