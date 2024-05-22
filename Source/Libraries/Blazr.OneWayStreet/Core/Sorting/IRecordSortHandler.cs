/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public interface IRecordSortHandler<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> AddSortsToQuery(IQueryable<TRecord> query, IEnumerable<SortDefinition> definitions);
}
