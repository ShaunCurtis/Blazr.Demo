/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public class WeatherForecastListQueryHandler
    : RecordListQueryHandlerBase<WeatherForecastListQuery, DvoWeatherForecast>
{
    private WeatherForecastListQuery? _listquery => this.listQuery as WeatherForecastListQuery;

    public WeatherForecastListQueryHandler(DbContext dbContext, WeatherForecastListQuery query)
        : base(dbContext, query)
    {}

    protected async override ValueTask<bool> GetItemsAsync()
    {
        if (_listquery is null)
            return false;

        IQueryable<DvoWeatherForecast> dbSet = this.dbContext.Set<DvoWeatherForecast>();

        if (_listquery.Request.PageSize > 0)
            dbSet = dbSet
                .Skip(_listquery.Request.StartIndex)
                .Take(_listquery.Request.PageSize);

        this.items = await dbSet.ToListAsync();

        return true;
    }
    
    protected async override ValueTask<bool> GetCountAsync()
    {
        if (_listquery is null)
            return false;

        IQueryable<DvoWeatherForecast> dbSet = this.dbContext.Set<DvoWeatherForecast>();

        count = await dbSet.CountAsync();
        return true;
    }
}
