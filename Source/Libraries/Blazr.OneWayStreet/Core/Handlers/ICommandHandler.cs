/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public interface ICommandHandler
{
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class;
}

public interface ICommandHandler<TRecord>
        where TRecord : class
{
    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<TRecord> request);
}

public interface ICommandHandler<TRecord, TDbo>
        where TRecord : class
        where TDbo : class
{
    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<TRecord> request);
}
