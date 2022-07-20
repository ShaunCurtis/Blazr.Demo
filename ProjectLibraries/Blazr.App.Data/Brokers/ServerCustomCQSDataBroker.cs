/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Data;

public class ServerCustomCQSDataBroker<TDbContext>
    :ICustomCQSDataBroker
    where TDbContext : DbContext
{
    protected readonly IDbContextFactory<TDbContext> factory;

    public ServerCustomCQSDataBroker(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    //public async ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastListQuery query)
    //{
    //    var handler = new WeatherForecastBySummaryListQueryHandler<TDbContext>(factory, query);
    //    return await handler.ExecuteAsync();
    //}
}
