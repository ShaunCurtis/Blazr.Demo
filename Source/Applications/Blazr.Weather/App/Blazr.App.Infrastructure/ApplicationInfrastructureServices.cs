using Microsoft.Extensions.DependencyInjection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class ApplicationInfrastructureServices
{
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

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryTestDbContext>>();

        if (factory is not null)
            TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);
    }

    public static void AddMappedWeatherForecastServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>, DboWeatherForecastMap>();
        services.AddScoped<IListRequestHandler<DmoWeatherForecast>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast>>();
        services.AddScoped<IItemRequestHandler<DmoWeatherForecast, Guid>, MappedItemRequestServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast, Guid>>();
        services.AddScoped<ICommandHandler<DmoWeatherForecast>, MappedCommandServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast>>();

        services.AddTransient<IRecordFilterHandler<DboWeatherForecast>, WeatherForecastFilterHandler>();
        services.AddTransient<IRecordSortHandler<DboWeatherForecast>, WeatherForecastSortHandler>();
    }

}
