#  Data Objects

When data is retrieved from a data source, it's a **copy**.  It's not a pointer to the source data to mutate as you wish: it's read only.

That's the philosophy behind `OneWayStreet`.  Data sets are represented as readonly objects.

Data is mutated through a controlled process.  Create an editable object from the readonly data object.  Edit that object and then derive a new version of the read only object to pass back through the command pipeline to update the data store.

Implementing readonly objects is simple in modern C# using `record` value based objects with `{ get; init; }` property definitions.

The core *WeatherForecast* model object can be defined like this:

```csharp
public sealed record DmoWeatherForecast: ICommandEntity
{
    public WeatherForecastId WeatherForecastId { get; init; } = new(Guid.Empty);
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; set; } = new(0);
    public string? Summary { get; set; }
}
```

I'm avoiding *Primitive Obsession* for the Id and temperature.  Both are defined as value objects.

```csharp
public readonly record struct WeatherForecastId : IEntityKey
{
    public Guid Value { get; init; }
    public object KeyValue => this.Value;

    public WeatherForecastId(Guid value)
        => this.Value = value;

    public static WeatherForecastId NewEntity
        => new(Guid.Empty);
}
```

And:

```csharp
public readonly record struct Temperature
{
    public decimal TemperatureC { get; init; }
    
    [JsonIgnore]
    public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);

    public Temperature() { }

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

Primitives are used here because that's how the data is stored in the  data store [a SQL database].

A mapper object provides the connection between the domain and infrastructure objects:

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

The mapper is used in the CQS handlers.

## The Edit Context

The application allows adding and editing of *WeatherForecast* records so we need an edit context.

```csharp
public sealed class WeatherForecastEditContext
{
    public DmoWeatherForecast BaseRecord { get; private set; }
    public bool IsDirty => this.BaseRecord != this.AsRecord;

    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public DmoWeatherForecast AsRecord =>
        this.BaseRecord with
        {
            Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now),
            Summary = this.Summary,
            Temperature = new(this.Temperature)
        };

    public WeatherForecastEditContext()
    {
        this.BaseRecord = new DmoWeatherForecast();
        this.Load(this.BaseRecord);
    }

    public WeatherForecastEditContext(DmoWeatherForecast record)
    {
        this.BaseRecord = record;
        this.Load(record);
    }

    public IDataResult Load(DmoWeatherForecast record)
    {
        var alreadyLoaded = this.BaseRecord.WeatherForecastId != WeatherForecastId.NewEntity;

        if (alreadyLoaded)
            return DataResult.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
        return DataResult.Success();
    }
}
```

The record edit context is the model in edit forms. It's the `EditContext` model and it's public properties are used by the edit controls. The `TrackState` attribute is used by the `EditStateTracker` control to identify which *model* properties to track.

`AsRecord` provides a `DmoWeatherForecast` object representing the current state, and can be compared with `BaseRecord` to detect the current edit state. `EditStateTracker` provides access to individual property state.  

## Edit Context Validation

The application uses *Fluent Validation*.  The validator runs against the `WeatherForecastEditContext`.

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

## Newing up Records

It's easy to new up a record like this `new()`, but it doesn't always get you what you want.

 `INewRecordProvider<TRecord>` abstracts the creation to a DI defined service.

Here's the `NewWeatherForecastProvider` which doesn't do anything that `new()` can't do.

```csharp
public class NewWeatherForecastProvider : INewRecordProvider<DmoWeatherForecast>
{
    public DmoWeatherForecast NewRecord()
    {
        return new DmoWeatherForecast() { WeatherForecastId = new(Guid.NewGuid()), Date = DateOnly.FromDateTime(DateTime.Now) };
    }
}
```

But consider creating an `InvoiceItem` in the Invoice application.

```csharp
public class NewInvoiceItemProvider : INewRecordProvider<DmoInvoiceItem>
{
    public InvoiceId InvoiceId { get; private set; } = InvoiceId.NewEntity;

    public void SetInvoiceContext(InvoiceId invoiceId)
        => this.InvoiceId = invoiceId;

    public DmoInvoiceItem NewRecord()
    {
        return new DmoInvoiceItem() { InvoiceId = this.InvoiceId };
    }
}
```

The service is defined *scoped* and within the *Invoice* edit context, the context sets the `InvoiceId' to the loaded invoice.     