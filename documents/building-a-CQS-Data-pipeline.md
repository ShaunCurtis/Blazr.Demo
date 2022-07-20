# Building A Succinct CQS Data Pipeline

The first time I came across CQS I though it might answer some issues I had with the Repository pattern.  My exitement quickly disipated when I discovered how many classes I needed to create to implement it.  My generic repository framework trumped it hands down.

I recently had cause to revisit CQS on an application re-write and decided to work on creating a more succinct version.  This article is about what I've managed to achieve.

## Test Data Set

Before going any further I want to introduce my test data set.  As this pipeline was designed as part of a Blazor project, the weather is the setting.

There are two primary "tables":

```csharp
public record DboWeatherSummary 
{
    [Key] public Guid WeatherSummaryId { get; init; } = Guid.Empty;
    public string Summary { get; init; } = string.Empty
}
```

```csharp
public record DboWeatherForecast 
{
    [Key] public Guid WeatherForecastId { get; init; } = Guid.Empty;
    public Guid WeatherSummaryId { get; init; } = Guid.Empty;
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
}
```

One data "View":

```csharp
public record DvoWeatherForecast : IRecord
{
    [Key] public Guid Id { get; init; }
    public Guid WeatherSummaryId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
    public string Summary { get; init; } = String.Empty;
}
```

And one Foreign Key "View":

```csharp
public record FkWeatherSummaryId : BaseFkListItem { }
```

The underlying interface and base class for all Foreign key fields:

```csharp
public interface IFkListItem
{
    [Key] public Guid Id { get; }
    public string Name { get; }
}

public record BaseFkListItem : IFkListItem
{
    [Key] public Guid Id { get; init; }
    public string Name { get; init; } = String.Empty;
}
```

Note:

1. All are immutable records.
2. The `Key` attribute is used to label the primary key.
3. Guids are used as Id fields.
4. `TemperatureF` has gone.  It's an internal calculated parameter.  We'll add it back in the business logic.
5. `Dbo` records map to database table objects.
6. `Dvo` records map to database view objects.

There's a `WeatherTestDataProvider` singleton object to generate and access a test data set.

## Results

We can classify the results qwe want back inot five categories:

1. A result containing a collection of objects.
2. A result containing a record we wanr to retrieve.
3. A result containing a count of the number of records in a dataset
4. A result cointaning the status iof a command to  


```csharp
public readonly struct ListProviderResult<TRecord>
{
    public IEnumerable<TRecord> Items { get; }
    public int TotalItemCount { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```
```csaharp
public readonly struct RecordProviderResult<TRecord>
{
    public TRecord? Record { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```
```csaharp
public readonly struct RecordCountProviderResult
{
    public int Count { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```
```csaharp
public readonly struct CommandResult
{
    public Guid NewId { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; }
    //....Constructors
}
```
```csaharp
public readonly struct FKListProviderResult
{
    public IEnumerable<IFkListItem> Items { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```



