# Repository Data Brokers

Data brokers provide the link between the *Core* and *Data* domains.

The application implements two types of data broker.

1. A set of data brokers that implement a generic Repository type pattern for CRUD and Read/List operations.
2. A set of data brokers that implement a generic CQS pattern.

This article covers the Repository pattern data pipeline.
 
## The Classic Generic Repository Pattern

The classic generic pattern that you will see looks like this:

```csharp
public interface IRepository<T>
 where TRecord: class, new();
{
......
}

public class BaseRepository<T>
 where TRecord: class, new();
{
......
}

```

And the DI services instances declared like this:

```csharp
services.AddScoped<IRepository<WeatherForecast>, BaseRepository<WeatherForecast>(); 
services.AddScoped<IRepository<WeatherReport>, BaseRepository<WeatherReport>(); 
services.AddScoped<IRepository<WeatherStation>, BaseRepository<WeatherStation>(); 
services.AddScoped<IRepository<WeatherLocation>, BaseRepository<WeatherLocation>(); 
//.... Ad Finitum for all your POCO classes mapping to your database tables and views
```

This implementation uses a different approach.  The service declaration looks like this:

```csharp
services.AddSingleton<IDataBroker, ServerInMemoryDataBroker<InMemoryWeatherDbContext>>();
```


Generics are implemented at the method level, not the class level.  The application uses a single Singleton DI `IDataBroker` for the application.  Each call passes the data class to the method.  EF functionality maps requests to the correct `DbSet` and runs the action against that `DbSet`.

The `IDataBroker` interface is defined in the *Core* domain, one or more implementations in the *Data* domain, and all calls in the data pipeline are made though the interface.

All methods:
1. Conform to CRS principles.
2. Use *unit of work* `DbContext` instances.
3. Are *async*.
4. Return `ValueTask` wrapped *Result* objects.


### IDataBroker

First the interface.

```csharp
public interface IDataBroker
{
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync<TRecord>(ListProviderRequest request) where TRecord: class, new();
    public ValueTask<RecordProviderResult<TRecord>> GetRecordAsync<TRecord>(Guid id) where TRecord : class, new();
    public ValueTask<RecordCountProviderResult> GetRecordCountAsync<TRecord>() where TRecord : class, new();
    public ValueTask<CommandResult> AddRecordAsync<TRecord>(TRecord record) where TRecord : class, new();
    public ValueTask<CommandResult> UpdateRecordAsync<TRecord>(TRecord record) where TRecord : class, new();
    public ValueTask<CommandResult> DeleteRecordAsync<TRecord>(TRecord record) where TRecord : class, new();
}
```
Note that all methods define a restrained generic `TRecord` that are a class and implement an empty constructor.

## Results

Result objects standardize method return values and provide a mechanism to return status information tpo the caller. Each is a readonly `struct`.  Query results contain the data requested.  The List result is structured for paging operations.

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

### Server Data Broker

`ServerDataBroker` is the `IDataBroker` implementation for Blazor Server and API web servers.

The class takes a generic `TDbContext` constrained as  `DbContext`.  This abstracts the data broker from a specific `DbContext` implementation.  The demo solution uses an in-memory EF database.


```csharp
public class ServerDataBroker<TDbContext>
    : IDataBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;
    private bool _success;
    private string? _message;

    public ServerDataBroker(IDbContextFactory<TDbContext> factory)
        => _factory = factory;
```

Next we implement the standard CRUD operations.  Each method gets an individual DbContext from the factory with `Using`. 

`GetRecordAsync` either uses the Id if the record has one (implements `IRecord`) or EF's `FindAsync` method.

```csharp
public async ValueTask<RecordProviderResult<TRecord>> GetRecordAsync<TRecord>(Guid id) where TRecord : class, new()
{
    var dbContext = _factory.CreateDbContext();
    dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    TRecord? record = null;

    // first check if the record implements IRecord.  If so we can do a cast and then do the query via the Id property directly 
    if ((new TRecord()) is IRecord)
        record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Id == id);

    // Try and use the EF FindAsync implementation
    if (record == null)
        record = await dbContext.FindAsync<TRecord>(id);

    if (record is null)
    {
        _message = "No record retrieved";
        _success = false;
    }
    return new RecordProviderResult<TRecord>(record, _success, _message);
}
```

`GetRecordCountAsync` gets the total record count.  It uses `CountAsync` to make the operation async.

```csharp
public async ValueTask<RecordCountProviderResult> GetRecordCountAsync<TRecord>() where TRecord : class, new()
{
    using var dbContext = _factory.CreateDbContext();
    dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    IQueryable<TRecord> query = dbContext.Set<TRecord>();
    var count = await query.CountAsync();
    return new RecordCountProviderResult(count);
}
```

The Add/Update/Delete methods use built in EF record management functionality.  This is the Add.

```csharp
public async ValueTask<CommandResult> AddRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
{
    using var dbContext = _factory.CreateDbContext();

    var id = GetRecordId<TRecord>(item);

    // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the record to
    dbContext.Add(item);

    // We should have added a single record so the return count should be 1
    return await dbContext.SaveChangesAsync() == 1
        ? new CommandResult(id, true, "Record Added")
        : new CommandResult(id, false, "Failed to Add Record");
}
```

`GetRecordsAsync` returns a `ItemsProviderResult` object: the paged collection and the total number of records for the query.  Two internal methods do the work.  The methods use the `IQueryable` functionality of the underlying `DbSet` to construct a query based on `ListProviderRequest` and finally execute the query to get the result.  The query action is wrapped in a `try`: Dynamic Linq will throw exceptions if it can't build a valid expression tree from the provided string expression.

If you don't understand the differences between `IEnumerable` and `IQueryable` then I strongly recommend you read up on the subject.  [Here's an article](https://www.codeproject.com/Articles/1240553/LINQ-Part-An-Introduction-to-IQueryable).

To summarise.  `IQueryable` consists of:
 - A queryable object - the `IQueryableProvider`, in our case a `DbSet`.
 - An Expression Tree - the `Expression` which defines the query we want to run.

Methods such as `Where` or `Take` modify the `Expression`: they don't run against the underlying dataset.   Actions such as `ToListAsync` or `CountAsync` run the `Expression` against the `IQueryableProvider` to create a result.  `GetRecordsAsync` builds the query by adding to the `Expression` and only actually executes the query against the data set at the end.


```csharp
public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
{
    _message = null;
    _success = true;
    var list = await this.GetItemsAsync<TRecord>(options);
    var count = await this.GetCountAsync<TRecord>(options);
    return new ListProviderResult<TRecord>(list, count, _success, _message);    
}

protected async ValueTask<IEnumerable<TRecord>> GetItemsAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
{
    using var dbContext = _factory.CreateDbContext();
    dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

    IQueryable<TRecord> query = dbContext.Set<TRecord>();

    if (!string.IsNullOrWhiteSpace(options.FilterExpression))
        query = query
                .Where(options.FilterExpression);

    if (options.PageSize > 0)
        query = query
            .Skip(options.StartIndex)
            .Take(options.PageSize);

    try
    {
        return await query.ToListAsync();
    }
    catch
    {
        _success = false;
        _message = "Error in Executing Query.  This is probably caused by an incompatible SortExpression or QueryExpression";
        return new List<TRecord>();
    }
}

protected async ValueTask<int> GetCountAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
{
    using var dbContext = _factory.CreateDbContext();
    dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

    IQueryable<TRecord> query = dbContext.Set<TRecord>();

    if (!string.IsNullOrWhiteSpace(options.FilterExpression))
        query = query
                .Where(options.FilterExpression);

    try
    {
        return await query.CountAsync();
    }
    catch
    {
        _success = false;
        _message = "Error in Executing Query.  This is probably caused by an incompatible SortExpression or QueryExpression";
        return 0;
    }
}
```

`GetFKListAsync` gets a list of `IFkListItems` that are principly used in select controls in edit forms.

```csharp
    public async ValueTask<FKListProviderResult> GetFKListAsync<TRecord>() where TRecord : class, IFkListItem, new()
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();
        var list = await query.ToListAsync();

        if (list is null)
            return new FKListProviderResult(Enumerable.Empty<IFkListItem>(), false, "Coukld not retrieve the FK List");

        var fklist = list.Cast<IFkListItem>();

        return new FKListProviderResult(fklist);
    }
```

The `IFkListItem` interface, `BaseFkListItem` and `FkWeatherSummary` implementation look like this:

```csharp
public interface IFkListItem
{
    public Guid Id { get; }
    public string Name { get; }
}

public record BaseFkListItem 
    : IFkListItem
{
    [Key] public Guid Id { get; init; }
    public string Name { get; init; } = String.Empty;
}

public record FkWeatherSummary : BaseFkListItem { }
```

## Testing

The application uses XUnit testing.

### Defining the Test DI Service Container

As this is a *system** rather than *unit* test we build out a DotNetCore DI container.

This is relatively simple:

1. Create a `ServiceCollection`.
2. Add services to the collection.
3. Create a DI container instance - called a `ServiceProvider` - by calling `ServiceCollection.BuildServiceProvider()`.

```csharp
public class RepositoryBrokerTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public RepositoryBrokerTests()
        => _weatherTestDataProvider = WeatherTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
        var provider = services.BuildServiceProvider();

        WeatherAppDataServices.AddTestData(provider);

        return provider!;
    }
    ///... tests
}
```

`AddInMemoryWeatherAppServerDataServices` is an extension method on `IServiceCollection`.  You can see it below.  It defines all the services we need to a specific application requirement.

`AddTestData` adds the test data to the in-memory database. 

```csharp
public static class WeatherAppDataServices
{
    public static void AddWeatherAppServerDataServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(options);
        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<IDataBroker, ServerDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<ICustomCQSDataBroker, ServerCustomCQSDataBroker<TDbContext>>();
    }

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>();

        if (factory is not null)
            WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherDbContext>(factory);
    }
}
```

We can then write a simple test to check the `GetRecordsAsync` like this:

```csharp
[Fact]
public async void TestRepositoryDataBrokerDboWeatherForecastList()
{
    var provider = GetServiceProvider();
    var broker = provider.GetService<IDataBroker>()!;

    var cancelToken = new CancellationToken();
    var listRequest = new ListProviderRequest(0, 1000, cancelToken);
    var result = await broker!.GetRecordsAsync<DboWeatherForecast>(listRequest);

    Assert.Equal(100, result.Items.Count());
}
```

And a `AddRecordAsync` test like this:

```csharp
[Fact]
public async void TestAddMovementRepositoryDataBroker()
{
    var provider = GetServiceProvider();
    var broker = provider.GetService<IDataBroker>()!;

    var testRecord = _weatherTestDataProvider.GetForecast();
    var newRecord = testRecord with { };
    var id = newRecord.WeatherForecastId;
    var result = await broker!.AddRecordAsync<DboWeatherForecast>(newRecord);

    var recordResult = await broker.GetRecordAsync<DboWeatherForecast>(id);

    Assert.True(result.Success);
    Assert.Equal(testRecord, recordResult.Record);
}
```