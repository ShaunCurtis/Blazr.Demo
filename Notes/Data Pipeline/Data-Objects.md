#  Data Objects

When we retrieve data from a data source it's a **copy** of the data within the data source.  It's not a pointer to the source data that you can mutate as you wish: it's read only.

You change the original by passing a mutated copy of the original into the data store.

Implementing readonly objects is simple in modern C# using `record` value based objects with `{ get; init; }` property definitions.

The core *WeatherForecast* model object can be defined like this:

```csharp
public sealed record DmoWeatherForecast : ICommandEntity
{
    public WeatherForecastId WeatherForecastId { get; init; } = new(Guid.Empty);
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; set; } = new(0);
    public string? Summary { get; set; }
}
```

Note I'm avoiding *Primitive Obsession* for the Id and temperature.  Both are defined as value objects.

```csharp
public sealed record WeatherForecastId(Guid Value) : IGuidKey;
```

And:

```csharp
public record Temperature
{
    private readonly decimal _temperature;
    public decimal TemperatureC => _temperature;
    public decimal TemperatureF => 32 + (_temperature / 0.5556m);

    public Temperature(decimal temperatureDebCelcius)
    {
        _temperature = temperatureDebCelcius;
    }
}
```

Stepping down into the *Infrastructure* domain, the database object is represented as `DmoWeatherForecast`:

```csharp
public sealed record DboWeatherForecast : ICommandEntity, IKeyedEntity
{
    [Key] public Guid WeatherForecastID { get; init; } = Guid.Empty;
    public DateTime Date { get; init; }
    public decimal Temperature { get; set; }
    public string? Summary { get; set; }

    public object KeyValue => this.WeatherForecastID;
}
```

Note that we have primitives here because that's how the data is stored in the defined data store [a SQL database].

To move between the two we define a mapper:

```csharp
public sealed class DboWeatherForecastMap : IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>
{
    public DmoWeatherForecast MapTo(DboWeatherForecast item)
        => Map(item);

    public DboWeatherForecast MapTo(DmoWeatherForecast item)
        => Map(item);

    public static DmoWeatherForecast Map(DboWeatherForecast item)
        => new()
        {
            WeatherForecastId = new(item.WeatherForecastID),
            Date = DateOnly.FromDateTime(item.Date),
            Temperature = new(item.Temperature),
            Summary = item.Summary
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            WeatherForecastID = item.WeatherForecastId.Value,
            Date = item.Date.ToDateTime(TimeOnly.MinValue),
            Temperature = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
```

We'll see that mapper used in the CQS handlers.

## The Edit Context

The application allows adding and editing of *WeatherForecast* records so we need an edit context.

```csharp
public sealed class WeatherForecastEditContext
{
    private DmoWeatherForecast _baseRecord;

    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public WeatherForecastId Id => _baseRecord.WeatherForecastId;
    public bool IsSummaryClean => Summary is not null ? Summary.Equals(_baseRecord.Summary) : true;
    public bool IsDirty => _baseRecord != this.AsRecord;

    public DmoWeatherForecast AsRecord =>
        _baseRecord with
        {
            Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now),
            Summary = this.Summary,
            Temperature = new(this.Temperature)
        };

    public WeatherForecastEditContext(DmoWeatherForecast record)
    {
        _baseRecord = record;
        this.Load(record);
    }

    public void Load(DmoWeatherForecast record)
    {
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
    }

    public void Reset()
        => this.Load(_baseRecord);
}
```

The record edit context is the model for `EditContext` and we point the edit controls to the public properties. The `TrackState` attribute tells the Edit State Tracker control which *model* properties to track.

Note that as `DmoWeatherForecast` is a value object we can use equality checking to track state. 

## Edit Context Validation

The application uses *Fluent Validation*.  The validator runs against the `WeatherForecastEditContext`,

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