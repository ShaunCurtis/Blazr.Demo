/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class DbContextTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public DbContextTests()
    {
        _weatherTestDataProvider = WeatherTestDataProvider.Instance();
    }

    private IDataBroker BuildServiceContainer()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<IDataBroker, ServerEFInMemoryDataBroker<InMemoryWeatherDbContext>>();
        var provider = services.BuildServiceProvider();

        return provider.GetService<IDataBroker>()!;
    }

    [Fact]
    public async void TestRepositoryDataBrokerDboWeatherForecastList()
    {
        var broker = this.BuildServiceContainer();

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 1000, cancelToken);
        var result = await broker!.GetRecordsAsync<DboWeatherForecast>(listRequest);

        Assert.Equal(100, result.Items.Count());
    }

    [Fact]
    public async void TestRepositoryDataBrokerDboWeatherSummaryList()
    {
        var broker = this.BuildServiceContainer();

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 1000, cancelToken);
        var result = await broker!.GetRecordsAsync<DboWeatherSummary>(listRequest);

        Assert.Equal(10, result.Items.Count());
    }


    [Fact]
    public async void TestAddMovementRepositoryDataBroker()
    {
        var broker = this.BuildServiceContainer();

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
        var broker = this.BuildServiceContainer();

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
        var broker = this.BuildServiceContainer();

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
