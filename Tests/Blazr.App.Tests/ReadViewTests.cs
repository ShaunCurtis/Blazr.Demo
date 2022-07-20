/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class ReadViewTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public ReadViewTests()
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

    [Fact]
    public async void TestGetDvoRecordDataBroker()
    {
        var services = GetServiceProvider();
        var view = services.GetService<IReadService<DvoWeatherForecast>>()!; ;

        var testRecord = _weatherTestDataProvider.GetDvoWeatherForecast(_weatherTestDataProvider.GetRandomRecord()!);
        var id = testRecord.Id;

        await view.GetRecordAsync(id);

        Assert.Equal(view.Record, testRecord);
    }

    [Fact]
    public async void TestGetDboRecordDataBroker()
    {
        var services = GetServiceProvider();
        var view = services.GetService<IReadService<DboWeatherForecast>>()!; ;

        var testRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var id = testRecord.WeatherForecastId;

        await view.GetRecordAsync(id);

        Assert.Equal(view.Record, testRecord);
    }
}
