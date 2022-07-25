/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Data;

public static class WeatherAppDataServices
{
    public static void AddWeatherAppServerDataServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(options);
        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<IDataBroker, ServerDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<ICustomCQSDataBroker, ServerCustomCQSDataBroker<TDbContext>>();

        //services.AddTransient<IListQueryHandler<DvoWeatherForecast>, ListQueryHandler<DvoWeatherForecast, InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DboWeatherLocation>, ListQueryHandler<DboWeatherLocation, InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DboUser>, ListQueryHandler<DboUser, InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();

        services.AddWeatherServices();
    }

    public static void AddWeatherAppWASMDataServices(this IServiceCollection services)
    {
        services.AddSingleton<ICQSDataBroker, CQSAPIDataBroker>();

        services.AddTransient<IListQueryHandler<DboWeatherLocation>, ListQueryHandler<DboWeatherLocation, InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DboUser>, ListQueryHandler<DboUser, InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();

        services.AddWeatherServices();
    }

    public static void AddWeatherAppWASMServerDataServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(options);
        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DboWeatherLocation>, ListQueryHandler<DboWeatherLocation, InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DboUser>, ListQueryHandler<DboUser, InMemoryWeatherDbContext>>();
        services.AddTransient<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();
    }


    public static void AddInMemoryWeatherRepositoryAppServerDataServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<IDataBroker, ServerDataBroker<InMemoryWeatherDbContext>>();
        services.AddSingleton<ICustomCQSDataBroker, ServerCustomCQSDataBroker<InMemoryWeatherDbContext>>();
    }

    public static void AddTestData<TDbContext>(IServiceProvider provider) where TDbContext : DbContext
    {
        var factory = provider.GetService<IDbContextFactory<TDbContext>>();

        if (factory is not null)
            WeatherTestDataProvider.Instance().LoadDbContext<TDbContext>(factory);
    }

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>();

        if (factory is not null)
            WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherDbContext>(factory);
    }
}
