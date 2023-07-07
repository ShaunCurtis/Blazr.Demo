/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface ICommandHandler
{
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class, IStateEntity, new();
}

public interface ICommandHandler<TRecord>
        where TRecord : class, IStateEntity, new()
{
    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<TRecord> request);
}
