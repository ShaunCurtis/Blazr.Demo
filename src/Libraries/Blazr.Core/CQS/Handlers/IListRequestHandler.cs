/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IListRequestHandler
{
    public ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class, new();
}

public interface IListRequestHandler<TRecord>
    where TRecord : class, new()
{
    public ValueTask<ListQueryResult<TRecord>> ExecuteAsync(ListQueryRequest request);
}
