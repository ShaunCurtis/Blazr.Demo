/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class QueryBrokerTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public QueryBrokerTests()
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
    public async void TestQueryBySummaryDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICustomCQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(0, 2, cancelToken);

        var summaryId = _weatherTestDataProvider.GetRandomRecord()?.WeatherSummaryId;

        var recordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherSummaryId == summaryId).Count();

        var query = new WeatherForecastBySummaryListQuery(summaryId, listRequest);
        var result = await broker.ExecuteAsync(query);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal(recordCount, result.TotalItemCount);
    }
}
