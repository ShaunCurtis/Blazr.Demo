/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public interface IListRequestHandler
{
    public ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class;
}

public interface IListRequestHandler<TRecord>
    where TRecord : class
{
    public ValueTask<ListQueryResult<TRecord>> ExecuteAsync(ListQueryRequest request);
}
