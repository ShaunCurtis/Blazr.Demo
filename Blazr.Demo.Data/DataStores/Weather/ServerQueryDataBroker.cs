/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class ServerQueryDataBroker<TDbContext>
    :IQueryDataBroker
    where TDbContext : DbContext
{
    protected readonly IDbContextFactory<TDbContext> database;

    public ServerQueryDataBroker(IDbContextFactory<TDbContext> db)
        => this.database = db;

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastListQuery query)
    {
        using var context = database.CreateDbContext();
        var handler = new WeatherForecastListQueryHandler(context, query);
        return await handler.ExecuteAsync();
    }

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastBySummaryListQuery query)
    {
        using var context = database.CreateDbContext();
        var handler = new WeatherForecastBySummaryListQueryHandler(context, query);
        return await handler.ExecuteAsync();
    }

    public async ValueTask<LookupListProviderResult> ExecuteAsync(WeatherSummaryLookupListQuery query)
    {
        using var context = database.CreateDbContext();
        var handler = new WeatherSummaryLookupListQueryHandler(context, query);
        return await handler.ExecuteAsync();
    }
}
