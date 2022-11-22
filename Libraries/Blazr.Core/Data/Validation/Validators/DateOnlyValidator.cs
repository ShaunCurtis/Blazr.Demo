/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class DateOnlyValidator : ValidatorBase<DateOnly>
{
    public DateOnlyValidator(DateOnly value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
    : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public DateOnlyValidator(DateOnly value, Guid objectUid, string fieldName, ValidationMessageCollection validationMessages, ValidationState validationState, string? message)
    : base(value, objectUid, fieldName, validationMessages, validationState, message) { }

    public DateOnlyValidator(DateOnly value, string? message = null)
    : base(value, message) { }

    public DateOnlyValidator LessThan(DateOnly test, string? message = null)
    {
        this.FailIfFalse(
            test: value < test,
            message: message);

        return this;
    }

    public DateOnlyValidator LessThanOrEqualTo(DateOnly test, string? message = null)
    {
        this.FailIfFalse(
            test: value <= test,
            message: message);

        return this;
    }

    public DateOnlyValidator GreaterThan(DateOnly test, bool dateOnly = false, string? message = null)
    {
        this.FailIfFalse(
            test: value > test,
            message: message);

        return this;
    }

    public DateOnlyValidator GreaterThanOrEqualTo(DateOnly test, bool dateOnly = false, string? message = null)
    {
        this.FailIfFalse(
            test: value >= test,
            message: message);

        return this;
    }

    public DateOnlyValidator NotDefault(string? message = null)
    {
        this.FailIfTrue(
            test: this.value == default(DateOnly),
            message: message);

        return this;
    }
}

public static class DateOnlyValidatorExtensions
{
    public static DateOnlyValidator Validation(this DateOnly value, string? message = null)
        => new DateOnlyValidator(value, message);

    public static DateOnlyValidator Validation(this DateOnly value, Guid objectUid, string fieldName, ValidationMessageCollection validationMessages, ValidationState validationState, string? message = null)
        => new DateOnlyValidator(value, objectUid, fieldName, validationMessages, validationState, message);

    public static DateOnlyValidator Validation(this DateOnly value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new DateOnlyValidator(value, fieldName, model, validationMessageStore, validationState, message);
}
