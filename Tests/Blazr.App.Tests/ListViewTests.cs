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
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
        services.AddWeatherForecastServices();
        var provider = services.BuildServiceProvider();

        WeatherAppDataServices.AddTestData(provider);

        return provider;
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
        var listRequest = new ListProviderRequest<DvoWeatherForecast>(startindex, pageSize, cancelToken);
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
}
