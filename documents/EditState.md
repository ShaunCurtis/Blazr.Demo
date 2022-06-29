# Managing Edit State and Validation


## Repository

You can find the code used in this article in my consolidated **Blazr.Demo** repository here: 

 - [Blazr.Demo Repository](https://github.com/ShaunCurtis/Blazr.Demo)

Note that this repo also contains a generic *Repository* framework data pipeline and several Blazor SPA implementations demonstrating various Blazor concepts and practices.

## The Edit Context

There are issues in many web applications with managing edit state.  

 - You can exit the application with unsaved data in a form.
 - You can hit the back button or a link half way through a edit session and lose all the edits or new entries you've made.
 - You can't reset the form.
 - You edit a value and change it back to the original and it still thinks you've edited it.
- you get some obtuse message on submit about something being wrong in the form.

This article is about to address all these issues in a Blazor application.

## Records

The first step to take is to recognise immutability.  

Any data retrieved from a data source should be immutable - you can't change it.  Your copy of the data/dataset represents the state of the data when you retrieved it.

DoteNetCore now has an immutable object - the `record`.  In the solution `DboWeatherForecast` is declared as follows:

```csharp
public record DboWeatherForecast 
{
    [Key] public Guid WeatherForecastId { get; init; } = Guid.Empty;
    public Guid WeatherSummaryId { get; init; } = Guid.Empty;
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
}
```

## The Edit Class

So how do you edit a Weather Forecast?

You create a editable objeect that contains the data that is editable.

```csharp
public class DeoWeatherForecast 
    : IEditRecord<DboWeatherForecast>
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid SummaryId { get; set; }
    public DateTimeOffset Date { get; set; }
    public int TemperatureC { get; set; }
    //.... more
}
```

We can now point edit controls to these properties.

Next we need to be able to quickly load data from the record.  We can do this in a `New` and a `Load` method.  Note that we keep a private copy of the original record used to load the class.

```csharp
private DboWeatherForecast _baseRecord = new DboWeatherForecast();

public DeoWeatherForecast() 
{ 
    var rec = new DboWeatherForecast();
    this.Load(rec);
}

public DeoWeatherForecast(DboWeatherForecast item)
    => this.Load(item);

public void Load(DboWeatherForecast record)
{
    _baseRecord = record with { };

    this.Id = record.WeatherForecastId;
    this.SummaryId = record.WeatherSummaryId;
    this.Date = record.Date;
    this.TemperatureC = record.TemperatureC;
}
```

And a way of getting a new record out with the edited values.  Note `AsNewRecord` sets the Id to a new Guid.

```csharp
private Guid _newId = Guid.NewGuid();

public class DeoWeatherForecast 
    : IEditRecord<DboWeatherForecast>
{
public DboWeatherForecast Record =>
    new DboWeatherForecast()
    {
        WeatherForecastId = this.Id,
        WeatherSummaryId = this.SummaryId,
        Date = this.Date,
        TemperatureC = this.TemperatureC
    };

public DboWeatherForecast AsNewRecord =>
    new DboWeatherForecast()
    {
        WeatherForecastId = _newId,
        WeatherSummaryId = this.SummaryId,
        Date = this.Date,
        TemperatureC = this.TemperatureC
    };
```

We can now do some state checks.  `IsDirty` is the most important.  We check the stored original copy of the record against a generated copy.  Using records means we can do a simple value based equality check.  

```csharp
public bool IsNull => Id == GuidExtensions.Null;
public bool IsNew => Id == Guid.Empty;
public bool IsDirty => _baseRecord != this.Record;
```

Finally we can implement validation.  This is custom validation covered below.

```csharp
public class DeoWeatherForecast 
    : IEditRecord<DboWeatherForecast>, IValidation
{

    //... code

    public bool Validate(ValidationMessageStore? validationMessageStore, string? fieldname, object? model = null)
    {
        model = model ?? this;
        bool trip = false;

        this.Date.Validation("Date", model, validationMessageStore)
            .LessThan(DateTime.Now.AddDays(10))
            .Validate(ref trip, fieldname);

        this.SummaryId.Validation("SummaryId", model, validationMessageStore)
            .NotEmpty("Guid can't be empty")
            .Validate(ref trip, fieldname);

        this.TemperatureC.Validation("TemperatureC", model, validationMessageStore)
            .GreaterThan(-61, "The minimum Temperatore is -60C")
            .LessThan(81, "The maximum temperature is 80C")
            .Validate(ref trip, fieldname);

        return !trip;
    }
}
```







 

 

