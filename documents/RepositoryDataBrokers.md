# Repository Data Brokers

## Data Brokers

Data brokers provide the link between the Core and Data domains.

The application has two types of data broker.

1. A set of data brokers that implement a generic Repository type pattern for CRUD and Read/List operations.
2. A set of data brokers that implement a generic CQS pattern.

This article covers the generic Repository pattern data pipeline,
 
## The Generic Repository Pattern

The classic generic pattern looks like this:

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
```

This implementation uses a different approach.

Generics are implemented at the method level, not the class level.  The application uses a single Singleton DI `IDataBroker` for the application.  Each call passes the data class to the method.  EF functionality maps requests to the correct `DbSet` and runs the action against that `DbSet`.  The command methods all return a simple `bool` status. 

The `IDataBroker` interface is defined in the core domain and all calls in the data pipeline are though this interface.

All methods conform with CRS principles and return *Result* objects.


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
Note:

1. All methods use generics.  We'll see how the implementation works in an implementation class.
2. All the methods are `async` and return a `ValueTask`.

## Results

Results standardize returns and provide a method to return status information on the request. Each is a `struct`.  Query results contain the data requested,  They're self explanatory.  The List result is structured for paging operations.

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

`ServerEFDataBroker` is the `IDataBroker` implementation for Blazor Server and API web servers.

Our service registration is as follows: 

```csharp
services.AddSingleton<IDataBroker, ServerInMemoryDataBroker<InMemoryWeatherDbContext>>();
```

The class takes a generic `TDbContext` constrained as  `DbContext`.  This abstracts the data broker from a specific `DbContext` implementation.  The demo solution uses an in-memory EF database.


```csharp
public class ServerEFDataBroker<TDbContext>
    : IDataBroker
    where TDbContext : DbContext
{
    protected readonly IDbContextFactory<TDbContext> factory;
    private bool _success;
    private string? _message;

    public ServerEFDataBroker(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;
```

All methods are async so we use "unit of work" Db contexts.

Next we implement the standard CRUD operations.  Each method gets an individual DbContext from the factory with `Using`. 

`GetRecordAsync` uses a helper method to get the `Index` attribute field and Dynamic Linq to run the query.

```csharp
    public async ValueTask<TRecord> GetRecordAsync<TRecord>(Guid id) where TRecord : class, new()
    {
        using var context = database.CreateDbContext();
        var key = GetKeyProperty<TRecord>();
        var dbSet = context.Set<TRecord>().AsNoTracking();
        if (dbSet is null)
            return new TRecord();

        var records = await dbSet
            .Where($"{key} == \"{id}\"")
            .ToListAsync();

        return records is not null && records.Count == 1
            ? records[0]
            : new TRecord();
    }

    private string GetKeyProperty<T>() where T : class, new()
    {
        var instance = new T();
        var prop = instance.GetType()
            .GetProperties()
            .FirstOrDefault(prop => prop.GetCustomAttributes(false)
                .OfType<KeyAttribute>()
                .Any());
        return prop?.Name ?? string.Empty;
    }
```

`GetRecordCountAsync` gets the total record count.  It uses `CountAsync` to make the operation async.

```csharp
    public async ValueTask<int> GetRecordCountAsync<TRecord>() where TRecord : class, new()
    {
        using var dbContext = database.CreateDbContext();
        IQueryable<TRecord> query = dbContext.Set<TRecord>();
        return await query.CountAsync();
    }
```

The Add/Update/Delete methods use built in EF record management functionality.

```csharp
public async ValueTask<bool> AddRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
{
    using var dbContext = database.CreateDbContext();
    dbContext.Add(item);
    return await dbContext.SaveChangesAsync() == 1;
}

public async ValueTask<bool> UpdateRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
{
    using var dbContext = database.CreateDbContext();
    dbContext.Update(item);
    return await dbContext.SaveChangesAsync() == 1;
}

public async ValueTask<bool> DeleteRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
{
    using var dbContext = database.CreateDbContext();
    dbContext.Remove(item);
    return await dbContext.SaveChangesAsync() == 1;
}
```
`GetRecordsAsync` returns a `ItemsProviderResult` object: the paged collection and the total number of records for the query.  Two internal methods do the work.  The methods use the `IQueryable` functionality of the underlying `DbSet` to construct a query based on `ListProviderRequest` and then execute the query to get the result.  The query action is wrapped in a `try`: Dynamic Linq will throw exceptions if it can't build a valid expression tree from the provided string expression.

If you don't understand the differences between `IEnumerable` and `IQueryable` then I strongly recommend you read up on the subject.  [Here's an article](https://www.codeproject.com/Articles/1240553/LINQ-Part-An-Introduction-to-IQueryable).

To summarise.  `IQueryable` consists of:
 - A queryable object - the `IQueryableProvider`, in our case a `DbSet`.
 - An Expression Tree - the `Expression` which defines the query we want to run.

Methods such as `Where` or `Take` modify the `Expression`: they don't run against the underlying dataset.   Actions such as `ToListAsync` or `CountAsync` run the `Expression` against the `IQueryableProvider` to create a result.  `GetRecordsAsync` builds the query by adding to the `Expression` and only actually executes the query against the data set at the end.


```csharp
public async ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
{
    var list = await this.GetItemsAsync<TRecord>(options);
    var count = await this.GetCountAsync<TRecord>(options);
    return new ItemsProviderResult<TRecord>(list, count);    
}

protected async ValueTask<IEnumerable<TRecord>> GetItemsAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
{
    using var dbContext = database.CreateDbContext();
    IQueryable<TRecord> query = dbContext.Set<TRecord>();
    if (!string.IsNullOrWhiteSpace(options.FilterExpression))
        query = query
                .Where(options.FilterExpression);

    if (options.Count > 0)
        query = query
            .Skip(options.StartIndex)
            .Take(options.Count);

    try
    {
        return await query.ToListAsync();
    }
    catch
    {
        return new List<TRecord>();
    }
}

protected async ValueTask<int> GetCountAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
{
    using var dbContext = database.CreateDbContext();
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
        return 0;
    }
}
```

### ServerInMemoryDataBroker

`ServerInMemoryDataBroker` is an `IDataBroker` implementation that uses an EF in-memory database context.  It inherits from `ServerEFDataBroker`.  The difference is New.  It uses the static class `WeatherForecastData` to generate test data and load it into the database.

```csharp
public class ServerInMemoryDataBroker<TDbContext>
    : ServerEFDataBroker<TDbContext>
    where TDbContext : DbContext, IWeatherDbContext
{
    private bool _initialized = false;

    public ServerInMemoryDataBroker(IDbContextFactory<TDbContext> db)
        : base(db)
    {
        // We need to populate this In Memory version of the database so we get a test data set from the static class WeatherForecastData
        if (!_initialized)
        {
            WeatherForecastData.Load();

            using var dbContext = database.CreateDbContext();

            // Check if we already have a full data set
            // If not clear down any existing data and start again
            if (dbContext.DboWeatherLocation.Count() == 0 || dbContext.DboWeatherForecast.Count() == 0)
            {
                dbContext.RemoveRange(dbContext.DboWeatherForecast.ToList());
                dbContext.RemoveRange(dbContext.DboWeatherLocation.ToList());
                dbContext.SaveChanges();
                dbContext.AddRange(WeatherForecastData.WeatherLocations);
                dbContext.AddRange(WeatherForecastData.WeatherForecasts);
                dbContext.SaveChanges();
            }
            _initialized = true;
        }
    }
}
```

## Testing

The application uses XUnit testing.

### Defining the Test DI Service Container

Creating a DotNetCore DI container is relatively simple:
1. Create a `ServiceCollection`.
2. Add services to the collection.
3. Create a container - called a `ServiceProvider` - by calling `ServiceCollection.BuildServiceProvider()`.

You can see the Test container creation in the test class New.  It adds the DbContext factory and the `ServerInMemoryDataBroker` instance of `IDataBroker`.

```csharp
public class DbContextTests
{
    private IServiceProvider _serviceProvider;
    private InMemoryDatabaseService? _testDatabaseData;

    public DbContextTests()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase("WeatherDatabase"));
        services.AddSingleton<IDataBroker, ServerInMemoryDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<InMemoryDatabaseService>();
        _serviceProvider = services.BuildServiceProvider();
        // instance initialised to create test data

        _testDatabaseData = _serviceProvider.GetService(typeof(InMemoryDatabaseService)) as InMemoryDatabaseService;
    }
}
```

We can then write a simple test to check the `GetRecordsAsync` like this:

```csharp
[Fact]
public async void TestListRepositoryDataBroker()
{
    var broker = _serviceProvider.GetService(typeof(IDataBroker)) as ServerInMemoryDataBroker<InMemoryWeatherDbContext>;
    var WeatherForecastBroker = _serviceProvider.GetService(typeof(IWeatherForecastDataBroker<InMemoryWeatherDbContext>)) as IWeatherForecastDataBroker<InMemoryWeatherDbContext>;

    var cancelToken = new CancellationToken();
    var listRequest = new ListProviderRequest(0, 10, cancelToken);
    var result = await broker!.GetRecordsAsync<DvoWeatherForecast>(listRequest);

    Assert.Equal(10, result.Items.Count());
    Assert.True(result.TotalItemCount >= 300);
}
```

And a `AddRecordAsync` test like this:

```csharp
[Fact]
public async void TestAddRepositoryDataBroker()
{
    var broker = _serviceProvider.GetService(typeof(IDataBroker)) as ServerInMemoryDataBroker<InMemoryWeatherDbContext>;
    var WeatherForecastBroker = _serviceProvider.GetService(typeof(IWeatherForecastDataBroker<InMemoryWeatherDbContext>)) as IWeatherForecastDataBroker<InMemoryWeatherDbContext>;

    var newRecord = _testDatabaseData!.GetForecast();
    var id = newRecord.WeatherForecastId;
    var result = await broker!.AddRecordAsync<DboWeatherForecast>(newRecord);

    var testRecord = await broker.GetRecordAsync<DboWeatherForecast>(id);

    Assert.True(result);
    Assert.Equal(newRecord, testRecord);
}
```