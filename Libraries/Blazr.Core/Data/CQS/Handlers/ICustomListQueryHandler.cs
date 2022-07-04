/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface ICustomListQueryHandler<TRecord>
        where TRecord : class, new()
{
    ValueTask<ListProviderResult<TRecord>> ExecuteAsync(ICustomListQuery<TRecord> query);
}
