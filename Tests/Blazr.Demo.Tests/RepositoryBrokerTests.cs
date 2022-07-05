
using System.Collections.Generic;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Tests;

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
        services.AddInMemoryWeatherAppServerDataServices();
        var provider = services.BuildServiceProvider();

        WeatherAppDataServices.AddTestData(provider);

        return provider!;
    }

    [Fact]
    public async void TestRepositoryDataBrokerDboWeatherForecastList()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest<DboWeatherForecast>(0, 1000, cancelToken);
        var result = await broker!.GetRecordsAsync<DboWeatherForecast>(listRequest);

        Assert.Equal(100, result.Items.Count());
    }

    [Fact]
    public async void TestRepositoryDataBrokerDboWeatherSummaryList()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest<DboWeatherSummary>(0, 1000, cancelToken);
        var result = await broker!.GetRecordsAsync<DboWeatherSummary>(listRequest);

        Assert.Equal(10, result.Items.Count());
    }

    [Fact]
    public async void TestRepositoryDataBrokerFKList()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var result = await broker!.GetFKListAsync<FkWeatherSummary>();

        Assert.Equal(10, result.Items.Count());
        Assert.IsType<FkWeatherSummary>(result.Items.First());
    }


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

    [Fact]
    public async void TestDeleteRepositoryDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var deleteRecord = _weatherTestDataProvider.GetRandomRecord()!;

        var oldCountResult = await broker!.GetRecordCountAsync<DboWeatherForecast>();

        var id = deleteRecord.WeatherForecastId;
        var result = await broker!.DeleteRecordAsync<DboWeatherForecast>(deleteRecord);

        var newCountResult = await broker!.GetRecordCountAsync<DboWeatherForecast>();
        var recordResult = await broker.GetRecordAsync<DboWeatherForecast>(id);

        Assert.True(result.Success);
        Assert.Null(recordResult.Record);
        Assert.Equal(oldCountResult.Count, newCountResult.Count + 1);

    }

    [Fact]
    public async void TestUpdateRepositoryDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var record = _weatherTestDataProvider.GetRandomRecord()!;
        var testRecord = record with { Date = record.Date.AddDays(1) };
        var updatedRecord = testRecord with { };

        var id = updatedRecord.WeatherForecastId;
        var result = await broker!.UpdateRecordAsync<DboWeatherForecast>(updatedRecord);

        var recordResult = await broker.GetRecordAsync<DboWeatherForecast>(id);

        Assert.True(result.Success);
        Assert.Equal(testRecord, recordResult.Record);
    }
}
