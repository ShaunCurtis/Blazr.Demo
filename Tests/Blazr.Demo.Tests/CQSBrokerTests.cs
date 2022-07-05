/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class CQSBrokerTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public CQSBrokerTests()
    {
        _weatherTestDataProvider = WeatherTestDataProvider.Instance();
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
        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 10, cancelToken);

        var query = new RecordListQuery<DvoWeatherForecast>(listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(10, result.Items.Count());
        Assert.True(result.TotalItemCount == 100);
    }

    [Fact]
    public async void TestCustomListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 2, cancelToken);

        var summaryId = _weatherTestDataProvider.GetRandomRecord()?.WeatherSummaryId;

        var recordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherSummaryId == summaryId).Count();

        var query = new WeatherForecastListQuery(summaryId, listRequest);

        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(recordCount, result.TotalItemCount);
    }

    [Fact]
    public async void TestFKCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var query = new FKListQuery<FkWeatherSummary>();
        var result = await broker.ExecuteAsync<FkWeatherSummary>(query);

        Assert.True(result.Success);
        Assert.Equal(_weatherTestDataProvider.WeatherSummaries.Count(), result.Items.Count());
    }

    [Fact]
    public async void TestRecordCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var testRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var CompareRecord = _weatherTestDataProvider.GetDvoWeatherForecast(testRecord);

        var query = new RecordGuidKeyQuery<DvoWeatherForecast>(testRecord.WeatherForecastId);
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
        var testRecord = _weatherTestDataProvider.WeatherSummaries.ToArray()[0]!;

        var query = new RecordGuidKeyQuery<DboWeatherSummary>(testRecord.WeatherSummaryId);
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

        var newRecord = _weatherTestDataProvider.GetForecast();
        var id = newRecord!.WeatherForecastId;

        var command = new AddRecordCommand<DboWeatherForecast>(newRecord);
        var result = await broker.ExecuteAsync(command);

        var query = new RecordGuidKeyQuery<DvoWeatherForecast>(id);
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

        var deleteRecord = _weatherTestDataProvider.GetRandomRecord()!;

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

        var editedRecord = _weatherTestDataProvider.GetRandomRecord()! with { Date = DateTime.Now.AddDays(10) };
        var editedDvoRecord = _weatherTestDataProvider.GetDvoWeatherForecast(editedRecord);
        var id = editedRecord.WeatherForecastId;

        var command = new UpdateRecordCommand<DboWeatherForecast>(editedRecord);
        var result = await broker.ExecuteAsync(command);

        var recordQuery = new RecordGuidKeyQuery<DvoWeatherForecast>(id);
        var updatedRecord = await broker.ExecuteAsync(recordQuery);

        Assert.True(result.Success);
        Assert.Equal(editedDvoRecord, updatedRecord.Record);
    }
}
