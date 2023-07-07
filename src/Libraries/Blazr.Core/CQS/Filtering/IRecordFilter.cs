/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IRecordFilter<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> AddFilterToQuery(IEnumerable<FilterDefinition> filters, IQueryable<TRecord> query)
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

    protected IPredicateSpecification<TRecord>? GetSpecification(FilterDefinition filter)
        => null;

}
