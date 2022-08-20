/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Data;

public static class WeatherAppDataServices
{
    /// <summary>
    /// This set of services is used by the full Blazor Server version of the Application
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="options"></param>
    public static void AddWeatherAppServerDataServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(options);
        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        services.AddScoped<IIdentityCQSHandler, IdentityCQSHandler<InMemoryWeatherDbContext>>();

        //services.AddTransient<IListQueryHandler<DvoWeatherForecast>, ListQueryHandler<DvoWeatherForecast, InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DboWeatherLocation>, ListQueryHandler<DboWeatherLocation, InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DboUser>, ListQueryHandler<DboUser, InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();

        services.AddWeatherServices();
    }

    /// <summary>
    /// This set of services is used by the Blazxor WASM client side version of the Application
    /// The data pipeline uses the HttpClient to make API calls to the WASM Application Server
    /// </summary>
    /// <param name="services"></param>
    public static void AddWeatherAppWASMDataServices(this IServiceCollection services)
    {
        // Set to scoped as it consumes the HttpClient service which is itself scoped.
        services.AddScoped<ICQSDataBroker, CQSAPIDataBroker>();
        services.AddScoped<ICQSAPIListHandlerFactory, CQSAPIListHandlerFactory>();

        services.AddScoped<IListQueryHandler<DboWeatherLocation>, ListQueryHandler<DboWeatherLocation, InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DboUser>, ListQueryHandler<DboUser, InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();

        services.AddWeatherServices();
    }

    public static void AddWeatherAppWASMServerDataServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(options);

        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        services.AddScoped<IIdentityCQSHandler, IdentityCQSHandler<InMemoryWeatherDbContext>>();

        services.AddScoped<IListQueryHandler<DboWeatherLocation>, ListQueryHandler<DboWeatherLocation, InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DboUser>, ListQueryHandler<DboUser, InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();

        services.AddWeatherServices();
    }


    public static void AddInMemoryWeatherRepositoryAppServerDataServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<IDataBroker, ServerDataBroker<InMemoryWeatherDbContext>>();
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
