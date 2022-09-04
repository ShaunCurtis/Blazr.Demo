/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class LongValidator : Validator<long>
{
    public LongValidator(long value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public LongValidator LessThan(long test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value < test,
            message: message);

        return this;
    }

    public LongValidator GreaterThan(long test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value > test,
            message: message);

        return this;
    }
}

public static class LongValidatorExtensions
{
    public static LongValidator Validation(this long value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new LongValidator(value, fieldName, model, validationMessageStore, validationState, message);
}
