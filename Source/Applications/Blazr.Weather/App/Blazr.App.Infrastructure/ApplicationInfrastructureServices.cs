using Microsoft.Extensions.DependencyInjection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class ApplicationInfrastructureServices
{
    /// <summary>
    /// Adds the server side Mapped Infrastructure services
    /// and generic handlers
    /// </summary>
    /// <param name="services"></param>
    public static void AddAppServerMappedInfrastructureServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<InMemoryTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        services.AddScoped<IDataBroker, DataBroker>();

        // Add the standard handlers
        services.AddScoped<IListRequestHandler, ListRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<IItemRequestHandler, ItemRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<ICommandHandler, CommandServerHandler<InMemoryTestDbContext>>();

        // Add any individual entity services
        services.AddMappedWeatherForecastServerInfrastructureServices();
    }

    /// <summary>
    /// Adds the generic services for the API Data Pipeline infrastructure
    /// </summary>
    /// <param name="services"></param>
    /// <param name="baseHostEnvironmentAddress"></param>
    public static void AddAppClientMappedInfrastructureServices(this IServiceCollection services, string baseHostEnvironmentAddress)
    {
        services.AddHttpClient();
        services.AddHttpClient(AppDictionary.Common.WeatherHttpClient, client => { client.BaseAddress = new Uri(baseHostEnvironmentAddress); });

        services.AddScoped<IDataBroker, DataBroker>();

        services.AddScoped<IListRequestHandler, ListRequestAPIHandler>();
        services.AddScoped<IItemRequestHandler, ItemRequestAPIHandler>();
        services.AddScoped<ICommandHandler, CommandAPIHandler>();

        services.AddAppClientMappedWeatherForecastInfrastructureServices();
    }

    /// <summary>
    /// Adds specific WeatherForecast API call implementations
    /// </summary>
    /// <param name="services"></param>
    /// <param name="baseHostEnvironmentAddress"></param>
    public static void AddAppClientMappedWeatherForecastInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<INewRecordProvider<DmoWeatherForecast>, NewWeatherForecastProvider>();

        //services.AddScoped<IListRequestHandler<DmoWeatherForecast>, WeatherForecastAPIListRequestHandler>();
        //services.AddScoped<IItemRequestHandler<DmoWeatherForecast, WeatherForecastId>, WeatherForecastAPIItemRequestHandler>();
        //services.AddScoped<ICommandHandler<DmoWeatherForecast>, WeatherForecastAPICommandHandler>();
    }

    /// <summary>
    ///  Adds the test data to the in-memory DB context
    /// </summary>
    /// <param name="provider"></param>
    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryTestDbContext>>();

        if (factory is not null)
            TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);
    }

    /// <summary>
    /// Adds the Server side Mapped Handlers
    /// </summary>
    /// <param name="services"></param>
    public static void AddMappedWeatherForecastServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>, DboWeatherForecastMap>();
        services.AddScoped<IListRequestHandler<DmoWeatherForecast>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast>>();
        services.AddScoped<IItemRequestHandler<DmoWeatherForecast, WeatherForecastId>, MappedItemRequestServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast, WeatherForecastId>>();
        services.AddScoped<ICommandHandler<DmoWeatherForecast>, MappedCommandServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast>>();

        services.AddTransient<IRecordFilterHandler<DboWeatherForecast>, WeatherForecastFilterHandler>();
        services.AddTransient<IRecordSortHandler<DboWeatherForecast>, WeatherForecastSortHandler>();

        services.AddScoped<INewRecordProvider<DmoWeatherForecast>, NewWeatherForecastProvider>();
    }

}
