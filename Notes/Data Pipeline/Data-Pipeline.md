#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

The primary implementation uses Entity Framework as the **Object Request Mapper**, overlayed by a thin organisational broker layer. Entity Framework implements the basic repository and unit of work patterns: there's no need for another fat layer. 

It's intent is to address persistence and retrieval in a standards based generic context with the flexibility to handle the more compkex aggregate cases with custom implementations.

## All data within the data pipeline is READONLY.

When you retrieve data from a data source it's a **copy** of the data within the data source.  It's not a pointer to the source data that you can mutate as you wish: it's read only.

You change the original by passing a mutated copy of the original into the data store.

Implementing readonly objects is simple in C# 8+ using `record` value based objects with `{ get; init; }` property definitions.

We can declare our core model data object like this:

```
public sealed record DmoWeatherForecast : ICommandEntity
{
    public WeatherForecastId WeatherForecastId { get; init; } = new(Guid.Empty);
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; set; } = new(0);
    public string? Summary { get; set; }
}
```

Core objects avoid *Primitive Obsession* where appropriate, so in the above record the identity is implemented as a value object:

```csharp
public sealed record WeatherForecastId(Guid Value) : IGuidKey;
```

And `Temperature` is treated the same:

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

The *Infrastructure* database object that represents `DmoWeatherForecast` looks like this:

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

We implement a mapper to map data between the two objects:

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

### Data Broker

We define the "contract" that the infrastructure domain code needs to honour as an `interface`.

```csharp
public interface IDataBroker
{
    public ValueTask<ListQueryResult<TRecord>> GetItemsAsync<TRecord>(ListQueryRequest request) where TRecord : class, new();
    public ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request) where TRecord : class, new();
    public ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request) where TRecord : class, new();
}
```

There are some key design points to take from this definition:

1. Generics are applied at the method level, not the class level.  There's one concrete implementation that implements the interface for all data classes.
2. Methods are passed a "Request" object and return a "Result" object.
3. Everything is Task based and returns a `ValueTask`.

The server implementation class looks like this.

For example the item request hander is defined as `IItemRequestHandler` and populated by DI in the constructor. `GetItemAsync` calls the `ExecuteAsync` method on the interface.  Each operation in the data broker has a corresponding handler defined by an interface and obtained through DI.


```csharp
public sealed class RepositoryDataBroker : IDataBroker
{
    private readonly IListRequestHandler _listRequestHandler;
    private readonly IItemRequestHandler _itemRequestHandler;
    private readonly ICommandHandler _commandHandler;

    public RepositoryDataBroker(IListRequestHandler listRequestHandler, IItemRequestHandler itemRequestHandler, CommandHandler commandHandler)
    {
        _listRequestHandler = listRequestHandler;
        _itemRequestHandler = itemRequestHandler;
        _commandHandler = commandHandler;
    }

    public ValueTask<ListQueryResult<TRecord>> GetItemsAsync<TRecord>(ListQueryRequest request) where TRecord : class, new()
        => _listRequestHandler.ExecuteAsync<TRecord>(request);

    public ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request) where TRecord : class, new()
        => _itemRequestHandler.ExecuteAsync<TRecord>(request);

    public ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request) where TRecord : class, new()
        => _commandHandler.ExecuteAsync<TRecord>(request);
}
```
