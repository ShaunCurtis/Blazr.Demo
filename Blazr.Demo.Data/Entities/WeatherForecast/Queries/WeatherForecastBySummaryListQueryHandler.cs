/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public class WeatherForecastBySummaryListQueryHandler<TDbContext>
    :ICQSHandler<WeatherForecastBySummaryListQuery, ValueTask<ListProviderResult<DvoWeatherForecast>>>
    where TDbContext : DbContext
{
    private WeatherForecastBySummaryListQuery _listQuery;
    private readonly IDbContextFactory<TDbContext> _factory;

    private IEnumerable<DvoWeatherForecast> _items = Enumerable.Empty<DvoWeatherForecast>();
    private int _count;
    private string _message = string.Empty;

    public WeatherForecastBySummaryListQueryHandler(IDbContextFactory<TDbContext> factory, WeatherForecastBySummaryListQuery query)
    {
        _listQuery = query;
        _factory = factory;
    }

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync()
    {
        if (await this.GetCountAsync())
            if (await this.GetItemsAsync())
                return new ListProviderResult<DvoWeatherForecast>(_items, _count, true, string.Empty);

        return new ListProviderResult<DvoWeatherForecast>(Enumerable.Empty<DvoWeatherForecast>(), 0, false, "Error retrieving records");
    }

    private async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = _factory.CreateDbContext();

        IQueryable<DvoWeatherForecast> dbSet = dbContext.Set<DvoWeatherForecast>();

        if (_listQuery.WeatherSummaryId is not null)
            dbSet = dbSet.Where(item => item.WeatherSummaryId == _listQuery.WeatherSummaryId);

        if (_listQuery.Request.PageSize > 0)
            dbSet = dbSet
                .Skip(_listQuery.Request.StartIndex)
                .Take(_listQuery.Request.PageSize);

        this._items = await dbSet.ToListAsync();
        return true;
    }
    
    private async ValueTask<bool> GetCountAsync()
    {
        var dbContext = _factory.CreateDbContext();

        IQueryable<DvoWeatherForecast> dbSet = dbContext.Set<DvoWeatherForecast>();

        if (_listQuery.WeatherSummaryId is not null)
            dbSet = dbSet.Where(item => item.WeatherSummaryId == _listQuery.WeatherSummaryId);

        this._count = await dbSet.CountAsync();

        return true;
    }
}
