# Command/Query Data Pipeline Pattern

This coding pattern implements the CQS methodology.

Each data action is either a:

1. *Command* - an instruction to make a data change.
2. *Query* - an instruction to get some data.

Each action has a Command/Query class that defines the action and a Handler class to execute the action.

## Queries

Queries implement an interface:

```csharp
public interface IRecordQuery<out TResponse> {}
```

`TResponse` is the obejct the query returns.

Here's the implementation for a WeatherForecast query that returns a `WeatherForecast` collection.

```csharp
public class PagedWeatherForecastsQuery : IRecordQuery<IEnumerable<WeatherForecast>>
{
    public ItemsProviderRequest Request { get; private set; }

    public PagedWeatherForecastsQuery(ItemsProviderRequest request)
        => Request = request;
}
```

`TResult` is defined as `IEnumerable<WeatherForecast>` and there's a `ItemsProviderRequest` property to page the collection, which is populated in the clss constructor.

The handler interface:

```csharp
public interface IRecordQueryHandler<in TQuery, TResponse>
    where TQuery : IRecordQuery<TResponse>
{
    ValueTask<TResponse> ExecuteAsync();
}
```

It takes an `IRecordQuery` and has a single method that returns the `TResponse` defined by the `IRecordQuery`.

Here's the handler for the `PagedWeatherForecastsQuery`:

```csharp
public class PagedWeatherForecastHandler
    : IRecordQueryHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private IDbContextFactory<InMemoryWeatherDbContext> _dbContextFactory { get; set; }
    private PagedWeatherForecastsQuery _query;

    public PagedWeatherForecastHandler(IDbContextFactory<InMemoryWeatherDbContext> factory, PagedWeatherForecastsQuery query )
    { 
        _dbContextFactory = factory;
        _query = query;
    }

    public async ValueTask<IEnumerable<WeatherForecast>> ExecuteAsync()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.WeatherForecast is not null
            ? await context.WeatherForecast
                .Skip(_query.Request.StartIndex)
                .Take(_query.Request.Count)
                .ToListAsync() 
            : new List<WeatherForecast>();
   }
}
```

1. It requires an `IDbContextFactory` to run it's query.
2. The constructor requires the `PagedWeatherForecastsQuery` instance and the `IDbContextFactory` instance.
3. `ExecuteAsync` runs the query against the DbContext's DbSet based on the query data provided - in this case the `ItemsProviderRequest` and returns the resulting `IEnumerable<WeatherForecast>`.

## Commands

Commands by definition only return status information so we can define a `CommandResponse` class.  Yes I know, I've broken the rule I just quoted: I've returned an Id.  This is an acceptable exception.  If you insert records with a database generated Id, the caller needs to know the Id of that record.  The response is the obvious place to provide that information, so we do so.  

```csaharp
public class CommandResponse
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}
``` 

`IRecordCommand` is a copy of `IRecordQuery`.

```csharp
public interface IRecordCommand<out TResult> { }
```

The Update a WeatherForecast implementation.

```csharp
public class UpdateWeatherForecastCommand : IRecordCommand<CommandResponse>
{
    public WeatherForecast Record { get; private set; }

    public UpdateWeatherForecastCommand(WeatherForecast item)
        => this.Record = item;
}
```

`IRecordCommandHandler` is a copy of `IQueryCommandHandler`

```csharp
public interface IRecordCommandHandler<in TCommand, TResult> where TCommand : IRecordCommand<TResult>
{
    ValueTask<TResult> ExecuteAsync();
}
```


And `UpdateWeatherForecastHandler`.

```csharp
public class UpdateWeatherForecastHandler
    : IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse>
{
    private IDbContextFactory<InMemoryWeatherDbContext> _dbContextFactory { get; set; }
    private UpdateWeatherForecastCommand _command;

    public UpdateWeatherForecastHandler(IDbContextFactory<InMemoryWeatherDbContext> factory, UpdateWeatherForecastCommand command )
    { 
        _dbContextFactory = factory;
        _command = command;
    }

    public async ValueTask<CommandResponse> ExecuteAsync()
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Update(_command.Record);
        var count = await context.SaveChangesAsync();

        return count == 1
            ? new CommandResponse { Id = _command.Record.Id, Message = "Record Saved", Success = true }
            : new CommandResponse { Id = _command.Record.Id, Message = "Error saving Record", Success = false };
   }
}
```

### Where do the classes belong?

`IRecordQuery`, `IRecordQueryHandler`, `IRecordCommandQuery` and `IRecordCommandHandler` are interfaces and used by the Core domain, so reside in Core domain.  The Query and Command implementions also reside in the Core domain.  The Handler implementations require access to the data source so reside in the Data domain.

### Data Service

There are a couple of problems with this implementation.  

1. A consumer in the Core domain can't initialize a handler because it doesn't know about data stores.
2. How does this work in a Blazor Web Assembly application?

We solve this problem using a factory.

First define an interface in the core:

```
public interface IWeatherForecastDataFactory
{
    IRecordQueryHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>> GetPagedRecordsQueryHandler(PagedWeatherForecastsQuery query);

    IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse> UpdateRecordCommandHandler(UpdateWeatherForecastCommand command);
}
```

We can now define our Server implementation.

```csharp
public class WeatherForecastDataFactory : IWeatherForecastDataFactory
{
    private readonly IDbContextFactory<InMemoryWeatherDbContext> _dbContext;

    public WeatherForecastDataFactory(IDbContextFactory<InMemoryWeatherDbContext> dbContext)
       => _dbContext = dbContext;

    public IRecordQueryHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>> GetPagedRecordsQueryHandler(PagedWeatherForecastsQuery query)
        => new PagedWeatherForecastHandler(_dbContext, query);

    public IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse> UpdateRecordCommandHandler(UpdateWeatherForecastCommand command)
        => new UpdateWeatherForecastHandler(_dbContext, command);
}
```

The factory provides a method to provide an interface based handler for each query/command.

We can then use the handler in any view service:

```csharp
public class WeatherForecastViewService : ViewServiceBase<WeatherForecast>
{
    private readonly IWeatherForecastDataFactory _weatherForecastDataFactory;
    public WeatherForecastViewService(IDataBroker dataBroker, IWeatherForecastDataFactory dataService)
        : base(dataBroker)
    { 
        _weatherForecastDataFactory = dataService;
    }

    public override async ValueTask GetRecordsAsync(ListOptions options)
    {
        var cancel = new CancellationToken();
        var request = new ItemsProviderRequest(options.StartIndex, options.PageSize, cancel);
        // Gets the record collection
        var handler =  _weatherForecastDataFactory.GetPagedRecordsQueryHandler(new PagedWeatherForecastsQuery(request));
        this.Records = await handler.ExecuteAsync() ;
    }
}
```

## API Versions

The `PagedWeatherForecastHandler` API version.  It uses the provided `HttpClient`.

```csharp
public class PagedWeatherForecastAPIHandler
    : IRecordQueryHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private PagedWeatherForecastsQuery _query;
    private HttpClient _httpClient;

    public PagedWeatherForecastAPIHandler(HttpClient httpClient, PagedWeatherForecastsQuery query )
    { 
        _httpClient = httpClient;
        _query = query;
    }

    public async ValueTask<IEnumerable<WeatherForecast>> ExecuteAsync()
    {
        IEnumerable<WeatherForecast>? result = null;
        var response = await _httpClient.PostAsJsonAsync<ItemsProviderRequest>($"/api/weatherforecast/list/", _query.Request);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
        return result ?? new List<WeatherForecast>();
    }
}
```

The `UpdateWeatherForecastHandler` API version.  It uses the provided `HttpClient`.

```csharp
public class UpdateWeatherForecastAPIHandler
    : IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse>
{
    private readonly HttpClient _httpClient;
    private readonly UpdateWeatherForecastCommand _command;

    public UpdateWeatherForecastAPIHandler(HttpClient httpClient, UpdateWeatherForecastCommand command )
    { 
        _httpClient = httpClient;
        _command = command;
    }

    public async ValueTask<CommandResponse> ExecuteAsync()
    {
        CommandResponse? result = null;
        var response = await _httpClient.PostAsJsonAsync<WeatherForecast>($"/api/add/", _command.Record);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResponse>();

        return result 
            ?? new CommandResponse { Id = Guid.Empty, Message = "Error updating Record", Success = false };
   }
}
```

And the API Factory:

```csharp
public class WeatherForecastAPIDataFactory : IWeatherForecastDataFactory
{
    private readonly HttpClient _httpClient;

    public WeatherForecastAPIDataService(HttpClient httpClient)
       => _httpClient = httpClient;

    public IRecordQueryHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>> GetPagedRecordsQueryHandler(PagedWeatherForecastsQuery query)
        => new PagedWeatherForecastAPIHandler(_httpClient, query);

    public IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse> UpdateRecordCommandHandler(UpdateWeatherForecastCommand command)
        => new UpdateWeatherForecastAPIHandler(_httpClient, command);
}
```

