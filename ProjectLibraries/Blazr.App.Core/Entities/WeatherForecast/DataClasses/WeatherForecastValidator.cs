/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class WeatherForecastValidator
{
    public static ValidationResult Validate(DboWeatherForecast record, ValidationMessageCollection? validationMessages, FieldReference? field = null)
    {
        ValidationState validationState = new ValidationState();

        ValidationMessageCollection messages = validationMessages ?? new ValidationMessageCollection();

        FieldReference propertyField;

        if (field != null)
            validationMessages?.ClearMessages(field);

        propertyField = field ?? FieldReference.Create(WeatherForecastConstants.Date);

        if (field is null || WeatherForecastConstants.Date.Equals(field.FieldName))
            record.Date.Validation(propertyField, messages, validationState)
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now), true, "The weather forecast must be for a future date")
            .Validate(field);

        propertyField = field ?? FieldReference.Create(WeatherForecastConstants.SummaryId);

        if (field is null || WeatherForecastConstants.SummaryId.Equals(field.FieldName))
            record.WeatherSummaryId.Validation(propertyField, messages, validationState)
            .NotEmpty("You must select a weather summary")
            .Validate(field);

        propertyField = field ?? FieldReference.Create(WeatherForecastConstants.LocationId);

        if (field is null || WeatherForecastConstants.LocationId.Equals(field.FieldName))
            record.WeatherLocationId.Validation(propertyField, messages, validationState)
            .NotEmpty("You must select a location")
            .Validate(field);

        propertyField = field ?? FieldReference.Create(WeatherForecastConstants.TemperatureC);

        if (field is null || WeatherForecastConstants.TemperatureC.Equals(field.FieldName))
            record.TemperatureC.Validation(propertyField, messages, validationState)
            .GreaterThan(-61, "The minimum Temperatore is -60C")
            .LessThan(61, "The maximum temperature is 60C")
            .Validate(field);

        return new ValidationResult { ValidationMessages = messages, IsValid = validationState.IsValid };
    }
}
