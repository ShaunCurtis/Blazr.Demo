# Entity Objects

## Identity

Entities have identity: some unique way of identifying them.

In *OneWayStreet* they are defined as:

```csharp
public interface IEntityKey 
{ 
    public object KeyValue { get; }
}
```

In the solution weather forecasts are unique and defined in the application domain by a strongly typed value object based on a Guid.

```csharp
public sealed record WeatherForecastId : IEntityKey
{
    public Guid Value { get; init; }

    public object KeyValue => this.Value;

    public WeatherForecastId(Guid value)
        => this.Value = value;

    public static WeatherForecastId NewEntity
        => new(Guid.Empty);
}
```

## Immutability

All data objects are immutable.  They can't be changed once created.

They are implemented as records with `{get; init;}` property definitions.

Here's the `WeatherForecast` object.

```csharp
public sealed record DmoWeatherForecast: ICommandEntity
{
    public WeatherForecastId WeatherForecastId { get; init; } = new(Guid.Empty);
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; set; } = new(0);
    public string? Summary { get; set; }
}
```

## Primitive Obsession

The original `WeatherForecast` defined `TemperatureC` as an int, ans then TemperatureF as a readonly property.  There are far better ways of dealing with this.  The application defines a `Temperature` value type:

```csharp
public record Temperature
{
    public decimal TemperatureC { get; init; }
    public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);

    /// <summary>
    /// temperature should be provided in degrees Celcius
    /// </summary>
    /// <param name="temperature"></param>
    public Temperature(decimal temperature)
    {
        this.TemperatureC = temperature;
    }
}
```

## Editing

Data objects are edited through record edit contexts: classes containing the properties that can be edited.  Record edit contexts are the models for edit forms.  `AsRecord` provides a record obejct representing the current edit state.

```csharp
public sealed class WeatherForecastEditContext
{
    public DmoWeatherForecast BaseRecord { get; }

    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public WeatherForecastId Id => this.BaseRecord.WeatherForecastId;
    public bool IsDirty => this.BaseRecord != this.AsRecord;

    public DmoWeatherForecast AsRecord =>
        this.BaseRecord with
        {
            Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now),
            Summary = this.Summary,
            Temperature = new(this.Temperature)
        };

    public WeatherForecastEditContext(DmoWeatherForecast record)
    {
        this.BaseRecord = record;
        this.Load(record);
    }

    public void Load(DmoWeatherForecast record)
    {
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
    }

    public void Reset()
        => this.Load(this.BaseRecord);
}
```

## Validation

The application uses Fluent Validation.  Each edit object has a validator.  Note it's against the record edit context, not the  record.

```csharp
public class WeatherForecastEditContextValidator : AbstractValidator<WeatherForecastEditContext>
{
    public WeatherForecastEditContextValidator()
    {
        this.RuleFor(p => p.Summary)
            .MinimumLength(3)
            .WithState(p => p);

        this.RuleFor(p => p.Date)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Date must be in the future")
            .WithState(p => p);

        this.RuleFor(p => p.Temperature)
            .GreaterThanOrEqualTo(-60)
            .LessThanOrEqualTo(70)
            .WithState(p => p);
    }
}
```




