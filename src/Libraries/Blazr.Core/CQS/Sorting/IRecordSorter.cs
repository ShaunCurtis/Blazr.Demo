/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IRecordSorter<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> AddSortToQuery(IQueryable<TRecord> query, IEnumerable<SortDefinition> definitions);
}
