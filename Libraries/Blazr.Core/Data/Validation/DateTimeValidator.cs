/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class DateTimeValidator : Validator<DateTime>
{
    public DateTimeValidator(DateTime value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public DateTimeValidator LessThan(DateTime test, bool dateOnly = false, string? message = null)
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

    public DateTimeValidator GreaterThan(DateTime test, bool dateOnly = false, string? message = null)
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

    public DateTimeValidator NotDefault(string? message = null)
    {
        this.FailIfTrue(
            test: this.value == default(DateTime),
            message: message);

        return this;
    }
}

public static class DateTimeValidatorExtensions
{
    public static DateTimeValidator Validation(this DateTime value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new DateTimeValidator(value, fieldName, model, validationMessageStore, validationState, message);

    public static DateTimeValidator Validation(this DateTime? value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new DateTimeValidator(value ?? DateTime.MinValue, fieldName, model, validationMessageStore, validationState, message);
}
