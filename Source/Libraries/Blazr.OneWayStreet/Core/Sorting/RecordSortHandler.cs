/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public abstract class RecordSortHandler<TRecord> 
    where TRecord : class
{
    protected Expression<Func<TRecord, object>>? DefaultSorter { get; set; }
    protected bool DefaultSortDescending { get; set; }

    protected Dictionary<string, string> PropertyNameMap { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Adds the sort definitions defined in definitions to the IQueryable query
    /// </summary>
    /// <param name="query"></param>
    /// <param name="definitions"></param>
    /// <returns></returns>
    public IQueryable<TRecord> AddSortsToQuery(IQueryable<TRecord> query, IEnumerable<SortDefinition> definitions)
    {
        if (this.PropertyNameMap.Count() > 0)
            definitions = this.ApplyPropertyNameMap(definitions);

        if (definitions.Any())
        {
            foreach (var defintion in definitions)
                query = RecordSorterHelper.AddSort<TRecord>(query, defintion);

            return query;
        }

        query = AddDefaultSort(query);
        return query;
    }

    private IEnumerable<SortDefinition> ApplyPropertyNameMap(IEnumerable<SortDefinition> inDefinitions)
    {
        List<SortDefinition> sortDefinitions = new List<SortDefinition>();

        foreach (var def in inDefinitions)
        {
            SortDefinition newDef = def;
            if (PropertyNameMap.ContainsKey(def.SortField))
                newDef = def with { SortField = this.PropertyNameMap[def.SortField] };

            sortDefinitions.Add(newDef);
        }

        return sortDefinitions;
    }

    private IQueryable<TRecord> AddDefaultSort(IQueryable<TRecord> query)
    {
        if (this.DefaultSorter is not null)
        {
            query = this.DefaultSortDescending
            ? query.OrderByDescending(this.DefaultSorter)
            : query.OrderBy(this.DefaultSorter);
        }

        return query;
    }
}
