/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class IntValidator : ValidatorBase<int>
{
    public IntValidator(int value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public IntValidator(int value, FieldReference field, ValidationMessageCollection validationMessages, ValidationState validationState, string? message)
    : base(value, field, validationMessages, validationState, message) { }

    public IntValidator(int value, string? message = null)
    : base(value, message) { }

    public IntValidator LessThan(int test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value < test,
            message: message);

        return this;
    }

    public IntValidator GreaterThan(int test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value > test,
            message: message);

        return this;
    }
}

public static class IntValidatorExtensions
{
    public static IntValidator Validation(this int value, string? message = null)
        => new IntValidator(value, message);

    public static IntValidator Validation(this int value, FieldReference field, ValidationMessageCollection validationMessages, ValidationState validationState, string? message = null)
        => new IntValidator(value, field, validationMessages, validationState, message);

    public static IntValidator Validation(this int value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new IntValidator(value, fieldName, model, validationMessageStore, validationState, message);
}

