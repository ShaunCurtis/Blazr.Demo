using static System.Net.Mime.MediaTypeNames;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core.Validation;

public class StringValidator : Validator<string>
{
    public StringValidator(string value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState , string? message)
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public StringValidator LongerThan(int test, string? message = null)
    {
        this.FailIfFalse(
            test: string.IsNullOrEmpty(this.value) || !(this.value.Length > test),
            message: message);

        return this;
    }

    public StringValidator ShorterThan(int test, string? message = null)
    {
        this.FailIfFalse(
            test: string.IsNullOrEmpty(this.value) || !(this.value.Length < test),
            message: message);

        return this;
    }

    public StringValidator Matches(string pattern, string? message = null)
    {
        var result = false;

        if (!string.IsNullOrWhiteSpace(this.value))
        {
            var match = Regex.Match(this.value, pattern);
            if (match.Success && match.Value.Equals(this.value)) 
                result = true;
        }
        
        this.FailIfFalse(result, message);

        return this;
    }
}

public static class StringValidatorExtensions
{
    public static StringValidator Validation(this string value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new StringValidator(value, fieldName, model, validationMessageStore, validationState, message);
}
