/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class ListViewTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public ListViewTests()
    {
        _weatherTestDataProvider = WeatherTestDataProvider.Instance();
    }

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<IQueryDataBroker, ServerQueryDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<IDataBroker, ServerEFInMemoryDataBroker<InMemoryWeatherDbContext>>();
        services.AddScoped<INotificationService<WeatherForecastService>, StandardNotificationService<WeatherForecastService>>();
        services.AddScoped<IListService<DvoWeatherForecast>, StandardListService<DvoWeatherForecast, WeatherForecastService>>();
        var serviceProvider = services.BuildServiceProvider();

        var db = serviceProvider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>()!;
        _weatherTestDataProvider.LoadDbContext(db);

        return serviceProvider;
    }

    [Theory]
    [InlineData(0, 20, 20)]
    [InlineData(20, 20, 20)]
    [InlineData(60, 60, 40)]
    [InlineData(10, 100, 90)]
    [InlineData(100, 100, 0)]
    public async void TestGetListDataBroker(int startindex, int pageSize, int records)
    {
        var services = GetServiceProvider();
        var view = services.GetService<IListService<DvoWeatherForecast>>()!; ;
        var notificationService = services.GetService<INotificationService<WeatherForecastService>>();

        var cancelToken = new CancellationToken();
        var listRequest = new ListProviderRequest(startindex, pageSize, cancelToken);
        object? eventSender = null;
        PagingEventArgs? pagingEvent = null;
        
        notificationService!.ListUpdated += delegate (object? sender, EventArgs e)
        { eventSender = sender; };

        notificationService!.ListPaged += delegate (object? sender, PagingEventArgs e)
        { pagingEvent = e; };

        var result = await view.GetRecordsAsync(listRequest);

        Assert.Equal(records, result.Items.Count());
        Assert.Equal(100, result.TotalItemCount);
        // Check no listupdated event triggered
        Assert.Null(eventSender);
        // Check we have a paging event triggered
        Assert.NotNull(pagingEvent);
    }

    [Fact]
    public async void TestGetRecordDataBroker()
    {
        var services = GetServiceProvider();
        var view = services.GetService<IListService<DvoWeatherForecast>>()!; ;

        var testRecord = _weatherTestDataProvider.GetDvoWeatherForecast(_weatherTestDataProvider.GetRandomRecord()!);
        var id = testRecord.WeatherForecastId;

        await view.GetRecordAsync(id);

        Assert.Equal(view.Record, testRecord);
    }

}
