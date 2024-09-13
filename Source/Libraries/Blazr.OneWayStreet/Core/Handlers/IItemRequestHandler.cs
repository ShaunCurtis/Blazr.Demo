/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public interface IItemRequestHandler
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord, TKey>(ItemQueryRequest<TKey> request)
        where TRecord : class;
}

public interface IItemRequestHandler<TRecord>
    where TRecord : class
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync(ItemQueryRequest request);
}

public interface IItemRequestHandler<TRecord, IKey>
    where TRecord : class
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync(ItemQueryRequest<IKey> request);
}
