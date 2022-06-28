/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class WeatherForecastCQSDataBroker<TDbContext>
    :IWeatherForecastCQSDataBroker
    where TDbContext : DbContext, IWeatherDbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public WeatherForecastCQSDataBroker(IDbContextFactory<TDbContext> factory, IServiceProvider serviceProvider)
        => _factory = factory;

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> GetWeatherForecastsAsync(RecordListQuery<DvoWeatherForecast> query)
    {
        var handler = new RecordListQueryHandler<DvoWeatherForecast,TDbContext>(_factory, query);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<RecordProviderResult<DvoWeatherForecast>> GetWeatherForecastAsync(RecordQuery<DvoWeatherForecast> query)
    {
        var handler = new RecordQueryHandler<DvoWeatherForecast, TDbContext>(_factory, query);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> AddWeatherForecastAsync(AddRecordCommand<DboWeatherForecast> command)
    {
        var handler = new AddRecordCommandHandler<DboWeatherForecast, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> UpdateWeatherForecastAsync(UpdateRecordCommand<DboWeatherForecast> command)
    {
        var handler = new UpdateRecordCommandHandler<DboWeatherForecast, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> DeleteWeatherForecastAsync(DeleteRecordCommand<DboWeatherForecast> command)
    {
        var handler = new DeleteRecordCommandHandler<DboWeatherForecast, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }
}
