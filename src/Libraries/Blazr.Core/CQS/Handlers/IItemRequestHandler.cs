/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IItemRequestHandler
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, IIdentity, new();
}

public interface IItemRequestHandler<TRecord>
        where TRecord : class, IIdentity, new()
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync(ItemQueryRequest request);
}
