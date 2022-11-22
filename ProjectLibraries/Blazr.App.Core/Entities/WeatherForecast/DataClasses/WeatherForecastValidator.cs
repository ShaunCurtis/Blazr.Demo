/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class WeatherForecastValidator
{
    public static ValidationResult Validate(DboWeatherForecast record, ValidationMessageCollection? validationMessages, string? fieldname = null)
    {
        ValidationState validationState = new ValidationState();

        ValidationMessageCollection messages = validationMessages ?? new ValidationMessageCollection();

        //TODO - all this needs fixing

        //if (fieldname != null)
        //    validationMessages?.ClearMessages(fieldname);

        //if (WeatherLocationConstants.Location.Equals(fieldname) || fieldname is null)
        //    record.Location.Validation(WeatherLocationConstants.Location, messages, validationState)
        //        .LongerThan(2, "The location miust be at least 2 characters")
        //        .Validate(fieldname);

        return new ValidationResult { ValidationMessages = messages, IsValid = validationState.IsValid };
    }
}
