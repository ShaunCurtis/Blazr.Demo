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
        services.AddTransient<IFilteredListQueryHandler<DvoWeatherForecast>, ListQueryHandlerBase<DvoWeatherForecast, InMemoryWeatherDbContext>>();
        //services.AddTransient<ICustomListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();
        var provider = services.BuildServiceProvider();

        WeatherAppDataServices.AddTestData(provider);

        return provider!;
    }

    [Fact]
    public async void TestWeatherQueryHandler()
    {
        var provider = GetServiceProvider();
        var handler = provider.GetService<IFilteredListQueryHandler<DvoWeatherForecast>>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 2, cancelToken);

        var summaryId = _weatherTestDataProvider.GetRandomRecord()?.WeatherSummaryId;

        var recordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherSummaryId == summaryId).Count();

        var query = new WeatherForecastListQuery(summaryId, listRequest);
        var result = await handler.ExecuteAsync(query);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal(recordCount, result.TotalItemCount);
    }

    [Fact]
    public async void TestCustomWeatherQueryHandler()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 2, cancelToken);

        var summaryId = _weatherTestDataProvider.GetRandomRecord()?.WeatherSummaryId;

        var recordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherSummaryId == summaryId).Count();

        var query = new WeatherForecastListQuery(summaryId, listRequest);

        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(recordCount, result.TotalItemCount);
    }


    [Fact]
    public async void TestCustomFullListWeatherQueryHandler()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 2, cancelToken);

        var recordCount = _weatherTestDataProvider.WeatherForecasts.Count();

        var query = new WeatherForecastListQuery(Guid.Empty, listRequest);

        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(recordCount, result.TotalItemCount);
    }

    [Fact]
    public async void TestCustomListWeatherQueryHandler2()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 2, cancelToken);

        var summaryId = _weatherTestDataProvider.GetRandomRecord()?.WeatherSummaryId;
        var recordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherSummaryId == summaryId).Count();

        Func<DvoWeatherForecast, bool> expression = item => item.WeatherSummaryId == summaryId;
        var query = new FilteredListQuery<DvoWeatherForecast>(listRequest, expression);

        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(recordCount, result.TotalItemCount);
    }

}
