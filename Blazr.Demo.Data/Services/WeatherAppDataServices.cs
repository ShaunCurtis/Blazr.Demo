/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public static class WeatherAppDataServices
{
    public static void AddWeatherAppServerDataServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<IDataBroker, ServerEFInMemoryDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<ICustomCQSDataBroker, ServerCustomCQSDataBroker<InMemoryWeatherDbContext>>();
    }
}
