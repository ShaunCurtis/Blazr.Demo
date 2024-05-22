/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public abstract class RecordFilterHandler<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> AddFiltersToQuery(IEnumerable<FilterDefinition> filters, IQueryable<TRecord> query)
    {
        foreach (var filter in filters)
        {
            var specification = GetSpecification(filter);
            if (specification != null)
                query = specification.AsQueryAble(query);
        }

        if (query is IQueryable)
            return query;

        return query.AsQueryable();
    }

    public abstract IPredicateSpecification<TRecord>? GetSpecification(FilterDefinition filter);
}
