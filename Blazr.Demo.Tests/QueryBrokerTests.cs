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

    private ICustomCQSDataBroker GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<ICustomCQSDataBroker, ServerCustomCQSDataBroker<InMemoryWeatherDbContext>>();
        var provider = services.BuildServiceProvider();

        var db = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>()!;
        _weatherTestDataProvider.LoadDbContext(db);

        return provider.GetService<ICustomCQSDataBroker>()!;
    }

    [Fact]
    public async void TestQueryBySummaryDataBroker()
    {
        var broker = GetServiceProvider();

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
