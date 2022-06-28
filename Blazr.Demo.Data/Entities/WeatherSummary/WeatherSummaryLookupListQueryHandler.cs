/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class WeatherSummaryLookupListQueryHandler
    : FKListQueryHandler<WeatherSummaryLookupListQuery, DboWeatherSummary>
{
    private WeatherSummaryLookupListQuery? _listquery => this.listQuery as WeatherSummaryLookupListQuery;

    public WeatherSummaryLookupListQueryHandler(DbContext dbContext, WeatherSummaryLookupListQuery query)
        : base(dbContext, query)
    {}

    public async override ValueTask<FKListProviderResult> ExecuteAsync()
    {
        IQueryable<DboWeatherSummary> dbSet = this.dbContext.Set<DboWeatherSummary>();

        var list = await dbSet
            .Select(item => new { key = item.WeatherSummaryId, value = item.Summary })
            .ToDynamicListAsync();

        SortedDictionary<Guid, string> sortedDict = new();
        foreach (var kvp in list)
            sortedDict.Add(kvp.Key, kvp.Value);

        return new LookupListProviderResult(sortedDict, sortedDict.Count, true, String.Empty);
    }
}
