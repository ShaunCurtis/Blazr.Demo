/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazr.Test;

public class WeatherForecastTests
{
    private TestDataProvider _testDataProvider;

    public WeatherForecastTests()
        => _testDataProvider = TestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddAppServerMappedInfrastructureServices();
        services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        // get the DbContext factory and add the test data
        var factory = provider.GetService<IDbContextFactory<InMemoryTestDbContext>>();
        if (factory is not null)
            TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);

        return provider!;
    }

    //[Fact]
    //public async void GetAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();

    //    //Injects the data broker
    //    var broker = provider.GetService<IDataBroker>()!;

    //    // Get the test item and it's Id from the Test Provider
    //    var testItem = _testDataProvider.WeatherForecasts.First();
    //    var testUid = testItem.WeatherForecastUid;

    //    // Builds an item request instance and Executes the query against the broker
    //    var request = ItemQueryRequest.Create(testUid);
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);

    //    // check the query was successful
    //    Assert.True(loadResult.Successful);

    //    // get the returned record 
    //    var dbItem = loadResult.Item;

    //    // check it matches the test record
    //    Assert.Equal(testItem, dbItem);
    //}

    //[Theory]
    //[InlineData(0, 10)]
    //[InlineData(0, 50)]
    //[InlineData(5, 10)]
    //public async void GetForecastList(int startIndex, int pageSize)
    //{
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    // Get the total expected count and the first record of the page
    //    var testCount = _testDataProvider.WeatherForecasts.Count();
    //    var testFirstItem = _testDataProvider.WeatherForecasts.Skip(startIndex).First();

    //    // Create a request and execute it against the broker
    //    var request = new ListQueryRequest { PageSize = pageSize, StartIndex = startIndex };
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    // Check the results are as expected
    //    Assert.Equal(testCount, loadResult.TotalCount);
    //    Assert.Equal(pageSize, loadResult.Items.Count());
    //    Assert.Equal(testFirstItem, loadResult.Items.First());
    //}

    //[Fact]
    //public async void GetAFilteredForecastList()
    //{
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    // Set up the test data
    //    var pageSize = 2;
    //    var testSummary = "Warm";
    //    var testQuery = _testDataProvider.WeatherForecasts.Where(item => testSummary.Equals(item.Summary, StringComparison.CurrentCultureIgnoreCase));
    //    var testCount = testQuery.Count();
    //    var testFirstItem = testQuery.First();

    //    // define the filter to use
    //    var filterDefinition = new FilterDefinition(ApplicationConstants.WeatherForecast.FilterWeatherForecastsBySummary, "Warm");
    //    var filters = new List<FilterDefinition>() { filterDefinition };

    //    // Define the query and execute it against the broker
    //    var request = new ListQueryRequest { PageSize = pageSize, StartIndex = 0, Filters = filters };
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    // Test the results are as expected
    //    Assert.Equal(testCount, loadResult.TotalCount);
    //    Assert.Equal(pageSize, loadResult.Items.Count());
    //    Assert.Equal(testFirstItem, loadResult.Items.First());
    //}

    //[Fact]
    //public async void GetASortedForecastList()
    //{
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    var testCount = _testDataProvider.WeatherForecasts.Count();
    //    var testFirstItem = _testDataProvider.WeatherForecasts.Last();

    //    SortDefinition sort = new("Date", true);
    //    var sortList = new List<SortDefinition>() { sort }; 

    //    var request = new ListQueryRequest { PageSize = 10000, StartIndex = 0, Sorters = sortList };
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    Assert.Equal(testFirstItem, loadResult.Items.First());

    //    sort = new("Date", false);
    //    sortList = new List<SortDefinition>() { sort };

    //    request = new ListQueryRequest { PageSize = 100000, StartIndex = 0, Sorters = sortList };
    //    loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    Assert.Equal(testFirstItem, loadResult.Items.Last());
    //}

    //[Fact]
    //public async void UpdateAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    // Get a record id to edit
    //    var testItem = _testDataProvider.WeatherForecasts.First();
    //    var testUid = testItem.WeatherForecastUid;

    //    // Build an item query and execute it against the broker to get the record to edit
    //    var request = ItemQueryRequest.Create(testUid);
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);
    //    var dbItem = loadResult.Item!;

    //    // construct a recordEditContext for the record
    //    // Normally you would plug your edit form fields into this context
    //    // We just update the temperature
    //    var recordEditContext = new WeatherForecastEditContext(dbItem);
    //    recordEditContext.TemperatureC = recordEditContext.TemperatureC + 10;

    //    // In a real edit setting, you would be doing validation to ensure the
    //    // recordEditContext values are valid before attempting to save the record
    //    // Note that the validation is on the WeatherForecastEditContext, not WeatherForecast!
    //    var newItem = recordEditContext.AsRecord;

    //    // Create an update command and execute it against the broker
    //    var command = new CommandRequest<WeatherForecast>(newItem, CommandState.Update);
    //    var commandResult = await broker.ExecuteCommandAsync<WeatherForecast>(command);
    //    Assert.True(commandResult.Successful);

    //    // Get the updated record from the broker and test they are the same
    //    request = ItemQueryRequest.Create(testUid);
    //    loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);
    //    var dbNewItem = loadResult.Item!;
    //    Assert.Equal(newItem, dbNewItem);

    //    // Execute a list query against the data broker and check the count is still the same
    //    // i.e. we haven't added a record instead of updating one
    //    var queryRequest = new ListQueryRequest { PageSize = 10, StartIndex = 0 };
    //    var queryResult = await broker.ExecuteQueryAsync<WeatherForecast>(queryRequest);
    //    Assert.True(queryResult.Successful);

    //    var testCount = _testDataProvider.WeatherForecasts.Count();
    //    Assert.Equal(testCount, queryResult.TotalCount);
    //}

    //[Fact]
    //public async void DeleteAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    // get the test record
    //    var testItem = _testDataProvider.WeatherForecasts.First();
    //    var testUid = testItem.WeatherForecastUid;
    //    var testCount = _testDataProvider.WeatherForecasts.Count() - 1;

    //    // build a command and execute it against the database
    //    var command = new CommandRequest<WeatherForecast>(testItem, CommandState.Delete);
    //    var commandResult = await broker.ExecuteCommandAsync<WeatherForecast>(command);
    //    Assert.True(commandResult.Successful);

    //    // build a item request and ensure the record no longwer exists
    //    var request = ItemQueryRequest.Create(testUid);
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.False(loadResult.Successful);

    //    // build a list query and check we have one less rcord 
    //    var queryRequest = new ListQueryRequest { PageSize = 10, StartIndex = 0 };
    //    var queryResult = await broker.ExecuteQueryAsync<WeatherForecast>(queryRequest);
    //    Assert.True(queryResult.Successful);
    //    Assert.Equal(testCount, queryResult.TotalCount);
    //}

    //[Fact]
    //public async void AddAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    var testCount = _testDataProvider.WeatherForecasts.Count() + 1;

    //    // Create a new record
    //    var newItem = new WeatherForecast { WeatherForecastUid = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.Now), Summary = "Testing", TemperatureC = 30 };

    //    // Create a command and execute it against the broker
    //    var command = new CommandRequest<WeatherForecast>(newItem, CommandState.Add);
    //    var commandResult = await broker.ExecuteCommandAsync<WeatherForecast>(command);
    //    Assert.True(commandResult.Successful);

    //    // Create a item query, execute it against the broker and check the new record exists
    //    var request = ItemQueryRequest.Create(newItem.WeatherForecastUid);
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    var dbNewItem = loadResult.Item!;
    //    Assert.Equal(newItem, dbNewItem);

    //    // create a list query and check thr total count has increased by 1 
    //    var queryRequest = new ListQueryRequest { PageSize = 10, StartIndex = 0 };
    //    var queryResult = await broker.ExecuteQueryAsync<WeatherForecast>(queryRequest);
    //    Assert.True(queryResult.Successful);
    //    Assert.Equal(testCount, queryResult.TotalCount);
    //}
}
