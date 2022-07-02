/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public class WeatherForecastBySummaryListQueryHandler<TDbContext>
    :RecordListQueryHandler<DvoWeatherForecast, TDbContext>
    where TDbContext : DbContext
{
    private WeatherForecastBySummaryListQuery _customQuery;

    public WeatherForecastBySummaryListQueryHandler(IDbContextFactory<TDbContext> factory, WeatherForecastBySummaryListQuery query)
        : base(factory, query)
        => _customQuery = query;

    protected override IQueryable<DvoWeatherForecast> GetCustomQueries(IQueryable<DvoWeatherForecast> query)
    {
        query = query.Where(item => item.WeatherSummaryId == _customQuery.WeatherSummaryId);
        return query;
    }
}
