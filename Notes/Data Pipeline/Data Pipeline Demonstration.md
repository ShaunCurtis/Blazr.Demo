#  Exploring the Data Pipeline

The best way to explore the data pipeline is by sertting up and running tests.

## Demo Tests Class

The solution provides a **Singleton Pattern** test data provider to provide a test data set.

The `DemoTests` class gets the test data instance as a global field on in the class constructor.

```csharp
public class DemoTests
{
    private TestDataProvider _testDataProvider;

    public DemoTests()
        => _testDataProvider = TestDataProvider.Instance();
    //....
}
```

We need to construct a DI services container and load up the data pipeline services.

```csharp
    private ServiceProvider GetServiceProvider()
    {
        // Creates a new root level DI Services container
        var services = new ServiceCollection();

        // Adds the debug logger
        services.AddLogging(builder => builder.AddDebug());

        // Add the data pipeline services
        services.AddAppServerMappedInfrastructureServices();

        // creates the DI container
        var provider = services.BuildServiceProvider();

        // get the DbContext factory and adds the test data
        var factory = provider.GetService<IDbContextFactory<InMemoryTestDbContext>>();
        if (factory is not null)
            TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);

        return provider!;
    }
```

`AddAppServerMappedInfrastructureServices` is an `IServiceCollection` extension method that encapsulates all the individual service confifuration into a single convenient method.

This method:

1. Adds the In-Memory database using the context factory.
2. Adds the DataBroker.  This is a facade class for the individual handlers.
3. Adds the standared generic handlers.  These are scoped services.
4. Calls `AddMappedWeatherForecastServerInfrastructureServices` to add the specific handlers.
 
```csharp
    public static void AddAppServerMappedInfrastructureServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<InMemoryTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        services.AddScoped<IDataBroker, DataBroker>();

        // Add the standard handlers
        services.AddScoped<IListRequestHandler, ListRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<IItemRequestHandler, ItemRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<ICommandHandler, CommandServerHandler<InMemoryTestDbContext>>();

        // Add any individual entity services
        services.AddMappedWeatherForecastServerInfrastructureServices();
    }
```

`AddMappedWeatherForecastServerInfrastructureServices` defines the individual services.  This:
1. The mapping classes between the infrastructure and domain objects.
2. The List, Item and Command handlers.
3. The filter and sorting handlers
4. The new record provider.
   

```csharp
    public static void AddMappedWeatherForecastServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>, DboWeatherForecastMap>();
        services.AddScoped<IListRequestHandler<DmoWeatherForecast>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast>>();
        services.AddScoped<IItemRequestHandler<DmoWeatherForecast, WeatherForecastId>, MappedItemRequestServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast, WeatherForecastId>>();
        services.AddScoped<ICommandHandler<DmoWeatherForecast>, MappedCommandServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast>>();

        services.AddTransient<IRecordFilterHandler<DboWeatherForecast>, WeatherForecastFilterHandler>();
        services.AddTransient<IRecordSortHandler<DboWeatherForecast>, WeatherForecastSortHandler>();

        services.AddScoped<INewRecordProvider<DmoWeatherForecast>, NewWeatherForecastProvider>();
    }
```

## Our First Test - Get an Item

The class wire frame:

```csharp
[Fact]
public async Task GetAForecast()
{
    //...
}
```

Get the full stocked DI container

```csharp
var provider = GetServiceProvider();
```

Get the DataBroker instance from DI.

```csharp
var broker = provider.GetService<IDataBroker>()!;
```

Get a `WeatherForecastId` to qurt against the data source.  We do that by getting a record from the test data provider and abstracting the entity id.

```csharp
        var testDboItem = _testDataProvider.WeatherForecasts.First();
        var testRawId = testDboItem.WeatherForecastID;
        WeatherForecastId testId = new(testRawId);
```

Build a test item we can use to validate thw returned record.  We do this manually so we can test the mapper.

```csharp
var testItem = new DmoWeatherForecast
{
    WeatherForecastId = testId,
    Date = DateOnly.FromDateTime(testDboItem.Date),
    Summary = testDboItem.Summary,
    Temperature = new(testDboItem.Temperature)
};
```

Build an `ItemQueryRequest` to pass into the data pipeline:

```csharp
var request = ItemQueryRequest<WeatherForecastId>.Create(testId);
```

Execute the `ItemQueryRequest` against the data broker
```csharp
var loadResult = await broker.ExecuteQueryAsync<DmoWeatherForecast,WeatherForecastId>(request);
```

Run tests to assert we have a success result and the retrieved domain record is the same as the original test record.
```csharp
Assert.True(loadResult.Successful);

var dbItem = loadResult.Item;
Assert.Equal(testItem, dbItem);
```

## Getting lists of Items

This test demonstrates getting a paged list.  See the inline comments for commentary.

```csharp
[Theory]
[InlineData(0, 10)]
[InlineData(0, 50)]
[InlineData(5, 10)]
public async Task GetForecastList(int startIndex, int pageSize)
{
    var provider = GetServiceProvider();
    var broker = provider.GetService<IDataBroker>()!;

    // Get the total count and the first paged item from the test provider
    var testCount = _testDataProvider.WeatherForecasts.Count();
    var testFirstItem = DboWeatherForecastMap.Map(_testDataProvider.WeatherForecasts.Skip(startIndex).First());

    // Build the ListQueryRequest
    var request = new ListQueryRequest { PageSize = pageSize, StartIndex = startIndex };
    var loadResult = await broker.ExecuteQueryAsync<DmoWeatherForecast>(request);

    // Test we have success
    Assert.True(loadResult.Successful);
    // Total number of items
    Assert.Equal(testCount, loadResult.TotalCount);
    // The correct page size
    Assert.Equal(pageSize, loadResult.Items.Count());
    // The correct first item in the pages list
    Assert.Equal(testFirstItem, loadResult.Items.First());
}
```

## Getting a Filtered List

The Test:

```csharp
[Fact]
public async Task GetAFilteredForecastList()
{
    var provider = GetServiceProvider();
    var broker = provider.GetService<IDataBroker>()!;

    var pageSize = 2;
    var testSummary = "Warm";
    var testQuery = _testDataProvider.WeatherForecasts.Where(item => testSummary.Equals(item.Summary, StringComparison.CurrentCultureIgnoreCase));

    var testCount = testQuery.Count();
    var testFirstItem = DboWeatherForecastMap.Map(testQuery.First());

    var filterDefinition = new FilterDefinition(AppDictionary.WeatherForecast.WeatherForecastFilterBySummarySpecification, "Warm");
    var filters = new List<FilterDefinition>() { filterDefinition };
    var request = new ListQueryRequest { PageSize = pageSize, StartIndex = 0, Filters = filters };

    var loadResult = await broker.ExecuteQueryAsync<DmoWeatherForecast>(request);

    Assert.True(loadResult.Successful);
    Assert.Equal(testCount, loadResult.TotalCount);
    Assert.Equal(pageSize, loadResult.Items.Count());
    Assert.Equal(testFirstItem, loadResult.Items.First());
}
```

Filters are defined using the *Specification pattern*.  If you're only building SSR or Blazor Server applications you can pass filters as delegates in the request.  However, you can't do that over API calls> you can't serialize a Linq query delegate.  You need a different mechanism.

In the application I use predefined filters.  You pass the filter name and the matching data object over the API call.

In this case `WeatherForecastFilterBySummarySpecification` defines the expression to apply.  In this case the data is a simple string value, but can be a json object for more complex data.

```csharp
public class WeatherForecastFilterBySummarySpecification : PredicateSpecification<DboWeatherForecast>
{
    private string? _summary;

    public WeatherForecastFilterBySummarySpecification() { }

    public WeatherForecastFilterBySummarySpecification(FilterDefinition filter)
        => _summary = filter.FilterData;

    public override Expression<Func<DboWeatherForecast, bool>> Expression
        => item => item.Summary != null ? item.Summary.Equals(_summary) : false;
}
```

The filter handler applies the name to class mapping:

```csharp
public class WeatherForecastFilterHandler : RecordFilterHandler<DboWeatherForecast>, IRecordFilterHandler<DboWeatherForecast>
{
    public override IPredicateSpecification<DboWeatherForecast>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            AppDictionary.WeatherForecast.WeatherForecastFilterBySummarySpecification => new WeatherForecastFilterBySummarySpecification(filter),
            _ => null
        };
}
```

## Getting a Sorted List

The Test:

```csharp
    [Fact]
    public async Task GetASortedForecastList()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var testCount = _testDataProvider.WeatherForecasts.Count();
        var testFirstItem = DboWeatherForecastMap.Map(_testDataProvider.WeatherForecasts.Last());

        SortDefinition sort = new("Date", true);
        var sortList = new List<SortDefinition>() { sort };

        var request = new ListQueryRequest { PageSize = 10000, StartIndex = 0, Sorters = sortList };
        var loadResult = await broker.ExecuteQueryAsync<DmoWeatherForecast>(request);
        Assert.True(loadResult.Successful);

        Assert.Equal(testFirstItem, loadResult.Items.First());

        sort = new("Date", false);
        sortList = new List<SortDefinition>() { sort };

        request = new ListQueryRequest { PageSize = 100000, StartIndex = 0, Sorters = sortList };
        loadResult = await broker.ExecuteQueryAsync<DmoWeatherForecast>(request);
        Assert.True(loadResult.Successful);

        Assert.Equal(testFirstItem, loadResult.Items.Last());
    }
```

Sorters are defined by passing through the string name for the property.

The sort handler applies the correct field to sort property name using a property map for the base sorter code to build the expression.

```csharp
public class WeatherForecastSortHandler : RecordSortHandler<DboWeatherForecast>, IRecordSortHandler<DboWeatherForecast>
{
    public WeatherForecastSortHandler()
    {
        DefaultSorter = (item) => item.Date;
        DefaultSortDescending = true;
        PropertyNameMap = new Dictionary<string, string>()
            {
                {"Temperature.TemperatureC", "Temperature" },
                {"Temperature.TemperatureF", "Temperature" }
            };
    }
}
```

```csharp
public IQueryable<TRecord> AddSortsToQuery(IQueryable<TRecord> query, IEnumerable<SortDefinition> definitions)
{
    if (this.PropertyNameMap.Count() > 0)
        definitions = this.ApplyPropertyNameMap(definitions);

    if (definitions.Any())
    {
        foreach (var defintion in definitions)
            query = RecordSorterHelper.AddSort<TRecord>(query, defintion);

        return query;
    }

    query = AddDefaultSort(query);
    return query;
}
```

## Running a Command

The following test demonstrates how to add a new weather forecast.  There are also tests to update and delete items.

```csharp
    [Fact]
    public async Task AddAForecast()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var newItemGuid = Guid.NewGuid();
        var newItem = new DmoWeatherForecast { WeatherForecastId = new(newItemGuid), Date = DateOnly.FromDateTime(DateTime.Now), Summary = "Testing", Temperature = new(30) };

        var command = new CommandRequest<DmoWeatherForecast>(newItem, CommandState.Add);
        var commandResult = await broker.ExecuteCommandAsync<DmoWeatherForecast>(command);
        Assert.True(commandResult.Successful);

        var request = ItemQueryRequest<WeatherForecastId>.Create(newItem.WeatherForecastId);
        var loadResult = await broker.ExecuteQueryAsync<DmoWeatherForecast, WeatherForecastId>(request);
        Assert.True(loadResult.Successful);

        var dbNewItem = loadResult.Item!;

        Assert.Equal(newItem, dbNewItem);

        var testCount = _testDataProvider.WeatherForecasts.Count() + 1;

        var queryRequest = new ListQueryRequest { PageSize = 10, StartIndex = 0 };
        var queryResult = await broker.ExecuteQueryAsync<DmoWeatherForecast>(queryRequest);
        Assert.True(queryResult.Successful);

        Assert.Equal(testCount, queryResult.TotalCount);
    }
```