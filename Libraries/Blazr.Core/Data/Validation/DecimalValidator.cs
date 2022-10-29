/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class DecimalValidator : Validator<decimal>
{
    public DecimalValidator(decimal value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public DecimalValidator(decimal value, string fieldName, ValidationMessageCollection validationMessages, ValidationState validationState, string? message)
    : base(value, fieldName, validationMessages, validationState, message) { }

    public DecimalValidator(decimal value, string? message = null)
    : base(value, message) { }

    public DecimalValidator LessThan(decimal test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value < test,
            message: message);

        return this;
    }


    public DecimalValidator LessThanOrEqualTo(decimal test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value <= test,
            message: message);

        return this;
    }

    public DecimalValidator GreaterThan(decimal test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value > test,
            message: message);

        return this;
    }

    public DecimalValidator GreaterThanOrEqualTo(decimal test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value >= test,
            message: message);

        return this;
    }
}

public static class DecimalValidatorExtensions
{
    public static DecimalValidator Validation(this decimal value, string? message = null)
        => new DecimalValidator(value, message);

    public static DecimalValidator Validation(this decimal value, string fieldName, ValidationMessageCollection validationMessages, ValidationState validationState, string? message = null)
        => new DecimalValidator(value, fieldName, validationMessages, validationState, message);

    public static DecimalValidator Validation(this decimal value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new DecimalValidator(value, fieldName, model, validationMessageStore, validationState, message);
}

