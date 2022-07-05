/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IListQueryHandler<TRecord>
        where TRecord : class, new()
{
    ValueTask<ListProviderResult<TRecord>> ExecuteAsync(IFilteredListQuery<TRecord> query);
}
