/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public interface IRecordFilterHandler<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> AddFiltersToQuery(IEnumerable<FilterDefinition> filters, IQueryable<TRecord> query);

    public IPredicateSpecification<TRecord>? GetSpecification(FilterDefinition filter);
}
