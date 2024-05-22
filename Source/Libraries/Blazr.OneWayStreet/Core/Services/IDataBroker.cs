/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public interface IDataBroker
{
    public ValueTask<ListQueryResult<TRecord>> ExecuteQueryAsync<TRecord>(ListQueryRequest request) where TRecord : class;

    public ValueTask<ItemQueryResult<TRecord>> ExecuteQueryAsync<TRecord>(ItemQueryRequest request) where TRecord : class;

    public ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request) where TRecord : class;
}
