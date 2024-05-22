/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public sealed class ServerDataBroker : IDataBroker
{
    private readonly IListRequestHandler _listRequestHandler;
    private readonly IItemRequestHandler _itemRequestHandler;
    private readonly ICommandHandler _commandHandler;

    public ServerDataBroker(
        IListRequestHandler listRequestHandler,
        IItemRequestHandler itemRequestHandler,
        ICommandHandler commandHandler)
    {
        _listRequestHandler = listRequestHandler;
        _itemRequestHandler = itemRequestHandler;
        _commandHandler = commandHandler;
    }

    public ValueTask<ItemQueryResult<TRecord>> ExecuteQueryAsync<TRecord>(ItemQueryRequest request) where TRecord : class
        => _itemRequestHandler.ExecuteAsync<TRecord>(request);

    public ValueTask<ListQueryResult<TRecord>> ExecuteQueryAsync<TRecord>(ListQueryRequest request) where TRecord : class
        => _listRequestHandler.ExecuteAsync<TRecord>(request);

    public ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request) where TRecord : class
        => _commandHandler.ExecuteAsync<TRecord>(request);
}