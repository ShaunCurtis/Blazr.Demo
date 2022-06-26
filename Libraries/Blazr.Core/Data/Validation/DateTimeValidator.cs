/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class DateTimeValidator : Validator<DateTime>
{
    public DateTimeValidator(DateTime value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message) 
        : base(value, fieldName, model, validationMessageStore, message) { }

    /// <summary>
    /// Check of the value is less than test
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public DateTimeValidator LessThan(DateTime test, bool dateOnly = false, string? message = null)
    {
        if (dateOnly)
        {
            if (!(Value.Date < test.Date))
            {
                Trip = true;
                LogMessage(message);
            }
        }
        else
        {
            if (!(this.Value < test))
            {
                Trip = true;
                LogMessage(message);
            }
        }
        return this;
    }

    /// <summary>
    /// Check of the value is greater than test
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public DateTimeValidator GreaterThan(DateTime test, bool dateOnly = false, string? message = null)
    {
        if (dateOnly)
        {
            if (!(Value.Date > test.Date))
            {
                Trip = true;
                LogMessage(message);
            }
        }
        else
        {
            if (!(this.Value > test))
            {
                Trip = true;
                LogMessage(message);
            }
        }
        return this;
    }

    /// <summary>
    /// Check of the value is default
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public DateTimeValidator NotDefault(string? message = null)
    {
        if (this.Value == default(DateTime))
        {
            Trip = true;
            LogMessage(message);
        }
        return this;
    }
}

public static class DateTimeValidatorExtensions
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
    public static DateTimeValidator Validation(this DateTime value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message = null)
    {
        var validation = new DateTimeValidator(value, fieldName, model, validationMessageStore, message);
        return validation;
    }

    public static DateTimeValidator Validation(this DateTime? value, string fieldName, object model, ValidationMessageStore? validationMessageStore, string? message = null)
    {
        var validation = new DateTimeValidator(value ?? DateTime.MinValue, fieldName, model, validationMessageStore, message);
        return validation;
    }
}
