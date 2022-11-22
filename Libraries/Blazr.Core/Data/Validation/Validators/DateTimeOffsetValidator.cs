/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class DateTimeOffsetValidator : ValidatorBase<DateTimeOffset>
{
    public DateTimeOffsetValidator(DateTimeOffset value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
    : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public DateTimeOffsetValidator(DateTimeOffset value, FieldReference field, ValidationMessageCollection validationMessages, ValidationState validationState, string? message)
    : base(value, field, validationMessages, validationState, message) { }

    public DateTimeOffsetValidator(DateTimeOffset value, string? message = null)
    : base(value, message) { }

    public DateTimeOffsetValidator LessThan(DateTimeOffset test, bool dateOnly = false, string? message = null)
    {
        if (dateOnly)
        {
            this.FailIfFalse(
                test: value.Date < test.Date,
                message: message);

            return this;
        }

        this.FailIfFalse(
            test: value < test,
            message: message);

        return this;
    }

    public DateTimeOffsetValidator GreaterThan(DateTimeOffset test, bool dateOnly = false, string? message = null)
    {
        if (dateOnly)
        {
            this.FailIfFalse(
                test: value.Date > test.Date,
                message: message);

            return this;
        }

        this.FailIfFalse(
            test: value > test,
            message: message);

        return this;
    }

    public DateTimeOffsetValidator NotDefault(string? message = null)
    {
        this.FailIfTrue(
            test: this.value == default(DateTime),
            message: message);

        return this;
    }
}

public static class DateTimeOffsetValidatorExtensions
{
    public static DateTimeOffsetValidator Validation(this DateTimeOffset value, string? message = null)
        => new DateTimeOffsetValidator(value, message);

    public static DateTimeOffsetValidator Validation(this DateTimeOffset value, FieldReference field, ValidationMessageCollection validationMessages, ValidationState validationState, string? message = null) 
        => new DateTimeOffsetValidator(value, field, validationMessages, validationState, message);

    public static DateTimeOffsetValidator Validation(this DateTimeOffset value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new DateTimeOffsetValidator(value, fieldName, model, validationMessageStore, validationState, message);
}
