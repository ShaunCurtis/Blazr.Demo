/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class WeatherForecastListQueryHandler<TDbContext>
    : ICustomQueryHandler<DvoWeatherForecast>
        where TDbContext : DbContext
{
    protected IEnumerable<DvoWeatherForecast> items = Enumerable.Empty<DvoWeatherForecast>();
    protected int count = 0;

    protected IDbContextFactory<TDbContext> factory;
    protected WeatherForecastListQuery listQuery = default!;

    public WeatherForecastListQueryHandler(IDbContextFactory<TDbContext> factory)
    {
        this.factory = factory;
    }

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(ICustomListQuery<DvoWeatherForecast> query)
    {
        if (query is null || query is not WeatherForecastListQuery)
            return new ListProviderResult<DvoWeatherForecast>(new List<DvoWeatherForecast>(), 0, false, "No Query Defined");

        listQuery = (WeatherForecastListQuery)query;

        if (await this.GetItemsAsync())
            await this.GetCountAsync();

        return new ListProviderResult<DvoWeatherForecast>(this.items, this.count);
    }

    protected virtual async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<DvoWeatherForecast> query = dbContext.Set<DvoWeatherForecast>();
        if (listQuery.FilterExpression is not null)
            query = query
                .Where(listQuery.FilterExpression)
                .AsQueryable();

        if (listQuery.Request.PageSize > 0)
            query = query
                .Skip(listQuery.Request.StartIndex)
                .Take(listQuery.Request.PageSize);

        if (query is IAsyncEnumerable<DvoWeatherForecast>)
            this.items = await query.ToListAsync();
        else
            this.items = query.ToList();

        return true;
    }

    protected virtual async ValueTask<bool> GetCountAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<DvoWeatherForecast> query = dbContext.Set<DvoWeatherForecast>();
        if (listQuery.FilterExpression is not null)
            query = query.Where(listQuery.FilterExpression).AsQueryable();

        if (query is IAsyncEnumerable<DvoWeatherForecast>)
            count = await query.CountAsync();
        else
            count = query.Count();

        return true;
    }
}
