/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

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
        services.AddScoped<IIdentityQueryHandler, IdentityQueryHandler<InMemoryWeatherDbContext>>();
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddWeatherServices();
    }

    public static void AddInMemoryAppServerDataServices(this IServiceCollection services)
    {
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
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
