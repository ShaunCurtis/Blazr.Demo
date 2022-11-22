/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherLocationValidator
{
    public static ValidationResult Validate(DboWeatherLocation record, ValidationMessageCollection? validationMessages, FieldReference? field = null)
    {
        ValidationState validationState = new ValidationState();

        ValidationMessageCollection messages = validationMessages ?? new ValidationMessageCollection();

        FieldReference propertyField;

        if (field != null)
            validationMessages?.ClearMessages(field);

        propertyField = field ?? FieldReference.Create(WeatherLocationConstants.Location);

        if (field is null || WeatherLocationConstants.Location.Equals(field.FieldName))
            record.Location.Validation(propertyField, messages, validationState)
            .LongerThan(2, "The location miust be at least 2 characters")
            .Validate(field);

        return new ValidationResult { ValidationMessages = messages, IsValid = validationState.IsValid };
    }
}
