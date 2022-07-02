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

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
        var provider = services.BuildServiceProvider();

        WeatherAppDataServices.AddTestData(provider);

        return provider!;
    }

    [Fact]
    public async void TestListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 10, cancelToken);

        var query = new RecordListQuery<DvoWeatherForecast>(listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(10, result.Items.Count());
        Assert.True(result.TotalItemCount == 100);
    }

    [Fact]
    public async void TestFKCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var query = new FKListQuery<FkWeatherSummary>();
        var result = await broker.ExecuteAsync<FkWeatherSummary>(query);

        Assert.True(result.Success);
        Assert.Equal(_weatherTestData.WeatherSummaries.Count(), result.Items.Count());
    }

    [Fact]
    public async void TestRecordCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var testRecord = _weatherTestData.GetRandomRecord()!;
        var CompareRecord = _weatherTestData.GetDvoWeatherForecast(testRecord);

        var query = new RecordGuidQuery<DvoWeatherForecast>(testRecord.WeatherForecastId);
        var result = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.NotNull(result.Record);
        Assert.Equal(CompareRecord, result.Record!);
    }

    [Fact]
    public async void TestDboRecordCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        //var testRecord = _weatherTestData.GetRandomRecord()!;
        var testRecord = _weatherTestData.WeatherSummaries.ToArray()[0]!;

        var query = new RecordGuidQuery<DboWeatherSummary>(testRecord.WeatherSummaryId);
        //var query = new RecordQuery<DboWeatherForecast>(testRecord.WeatherForecastId);
        var result = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.NotNull(result.Record);
        Assert.Equal(testRecord, result.Record!);
    }


    [Fact]
    public async void TestAddCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var newRecord = _weatherTestData.GetForecast();
        var id = newRecord!.WeatherForecastId;

        var command = new AddRecordCommand<DboWeatherForecast>(newRecord);
        var result = await broker.ExecuteAsync(command);

        var query = new RecordGuidQuery<DvoWeatherForecast>(id);
        var testRecord = await broker.ExecuteAsync(query);
        var testRec = testRecord.Record!;
        var rec = new DboWeatherForecast
        {
            WeatherForecastId = testRec.Id,
            WeatherSummaryId = testRec.WeatherSummaryId,
            Date = testRec.Date,
            TemperatureC = testRec.TemperatureC
        };

        Assert.True(result.Success);
        Assert.Equal(newRecord, rec);
    }

    [Fact]
    public async void TestDeleteCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 1000, cancelToken);
        var query = new RecordListQuery<DvoWeatherForecast>(listRequest);

        var startRecords = await broker.ExecuteAsync(query);

        var deleteRecord = _weatherTestData.GetRandomRecord()!;

        var command = new DeleteRecordCommand<DboWeatherForecast>(deleteRecord);
        var id = deleteRecord.WeatherForecastId;
        var result = await broker.ExecuteAsync(command);

        var endRecords = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.Equal(startRecords.TotalItemCount - 1, endRecords.TotalItemCount);
    }

    [Fact]
    public async void TestUpdateCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 1000, cancelToken);
        var query = new RecordListQuery<DvoWeatherForecast>(listRequest);

        var startRecords = await broker.ExecuteAsync(query);

        var editedRecord = _weatherTestData.GetRandomRecord()! with { Date = DateTime.Now.AddDays(10) };
        var editedDvoRecord = _weatherTestData.GetDvoWeatherForecast(editedRecord);
        var id = editedRecord.WeatherForecastId;

        var command = new UpdateRecordCommand<DboWeatherForecast>(editedRecord);
        var result = await broker.ExecuteAsync(command);

        var recordQuery = new RecordGuidQuery<DvoWeatherForecast>(id);
        var updatedRecord = await broker.ExecuteAsync(recordQuery);

        Assert.True(result.Success);
        Assert.Equal(editedDvoRecord, updatedRecord.Record);
    }
}
