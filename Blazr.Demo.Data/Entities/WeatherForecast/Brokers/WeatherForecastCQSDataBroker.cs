/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class WeatherForecastCQSDataBroker<TContext>
    :IWeatherForecastCQSDataBroker
    where TContext : DbContext, IWeatherDbContext
{
    private readonly IDbContextFactory<TContext> _factory;

    public WeatherForecastCQSDataBroker(IDbContextFactory<TContext> factory, IServiceProvider serviceProvider)
        => _factory = factory;

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> GetWeatherForecastsAsync(WeatherForecastListQuery query)
    {
        using var dbContext = _factory.CreateDbContext();
        var handler = new WeatherForecastListQueryHandler(dbContext, query);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<RecordProviderResult<DvoWeatherForecast>> GetWeatherForecastAsync(RecordQuery<DvoWeatherForecast> query)
    {
        using var dbContext = _factory.CreateDbContext();
        var handler = new WeatherForecastQueryHandler(dbContext, query);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> AddWeatherForecastAsync(WeatherForecastCommand command)
    {
        using var dbContext = _factory.CreateDbContext();
        var handler = new AddWeatherForecastCommandHandler(dbContext, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> UpdateWeatherForecastAsync(WeatherForecastCommand command)
    {
        using var dbContext = _factory.CreateDbContext();
        var handler = new UpdateWeatherForecastCommandHandler(dbContext, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> DeleteWeatherForecastAsync(WeatherForecastCommand command)
    {
        using var dbContext = _factory.CreateDbContext();
        var handler = new DeleteWeatherForecastCommandHandler(dbContext, command);
        var result = await handler.ExecuteAsync();
        return result;
    }
}
