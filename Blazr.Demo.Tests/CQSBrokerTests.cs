/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class CQSBrokerTests
{
    private WeatherTestDataProvider _weatherTestData;

    public CQSBrokerTests()
    {
        _weatherTestData = WeatherTestDataProvider.Instance();
    }

    private IWeatherForecastCQSDataBroker GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<IWeatherForecastCQSDataBroker, WeatherForecastCQSDataBroker<InMemoryWeatherDbContext>>();
        var provider = services.BuildServiceProvider();

        var db = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>()!;
        _weatherTestData.LoadDbContext(db);

        return provider.GetService<IWeatherForecastCQSDataBroker>()!; ;
    }

    [Fact]
    public async void TestListCQSDataBroker()
    {
        var broker = GetServiceProvider();

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 10, cancelToken);

        var query = new WeatherForecastListQuery(null, listRequest);
        var result = await broker.GetWeatherForecastsAsync(query);

        Assert.True(result.Success);
        Assert.Equal(10, result.Items.Count());
        Assert.True(result.TotalItemCount >= 100);
    }

    [Fact]
    public async void TestRecordCQSDataBroker()
    {
        var broker = GetServiceProvider();

        var testRecord = _weatherTestData.GetRandomRecord()!;
        var CompareRecord = _weatherTestData.GetDvoWeatherForecast(testRecord);

        var query = new RecordQuery<DvoWeatherForecast>(testRecord.WeatherForecastId);
        var result = await broker.GetWeatherForecastAsync(query);

        Assert.True(result.Success);
        Assert.NotNull(result.Record);
        Assert.Equal(CompareRecord, result.Record!);
    }


    [Fact]
    public async void TestAddRepositoryDataBroker()
    {
        var broker = GetServiceProvider();

        var newRecord = _weatherTestData.GetForecast();
        var id = newRecord!.WeatherForecastId;

        var command = new WeatherForecastCommand(newRecord);
        var result = await broker.AddWeatherForecastAsync(command);

        var query = new RecordQuery<DvoWeatherForecast>(id);
        var testRecord = await broker.GetWeatherForecastAsync(query);
        var testRec = testRecord.Record!;
        var rec = new DboWeatherForecast
        {
            WeatherForecastId = testRec.WeatherForecastId,
            WeatherSummaryId = testRec.WeatherSummaryId,
            Date = testRec.Date,
            TemperatureC = testRec.TemperatureC
        };

        Assert.True(result.Success);
        Assert.Equal(newRecord, rec);
    }

    [Fact]
    public async void TestDeleteRepositoryDataBroker()
    {
        var broker = GetServiceProvider();

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 1000, cancelToken);
        var query = new WeatherForecastListQuery(null, listRequest);

        var startRecords = await broker.GetWeatherForecastsAsync(query);

        var deleteRecord = _weatherTestData.GetRandomRecord()!;

        var command = new WeatherForecastCommand(deleteRecord);
        var id = deleteRecord.WeatherForecastId;
        var result = await broker.DeleteWeatherForecastAsync(command);

        var endRecords = await broker.GetWeatherForecastsAsync(query);

        Assert.True(result.Success);
        Assert.Equal(startRecords.TotalItemCount - 1, endRecords.TotalItemCount);
    }

    [Fact]
    public async void TestUpdateRepositoryDataBroker()
    {
        var broker = GetServiceProvider();

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 1000, cancelToken);
        var query = new WeatherForecastListQuery(null, listRequest);

        var startRecords = await broker.GetWeatherForecastsAsync(query);

        var editedRecord = _weatherTestData.GetRandomRecord()! with { Date = DateTime.Now.AddDays(10) };
        var editedDvoRecord = _weatherTestData.GetDvoWeatherForecast(editedRecord);
        var id = editedRecord.WeatherForecastId;

        var command = new WeatherForecastCommand(editedRecord);
        var result = await broker.UpdateWeatherForecastAsync(command);

        var recordQuery = new RecordQuery<DvoWeatherForecast>(id);
        var updatedRecord = await broker.GetWeatherForecastAsync(recordQuery);

        Assert.True(result.Success);
        Assert.Equal(editedDvoRecord, updatedRecord.Record);
    }
}
