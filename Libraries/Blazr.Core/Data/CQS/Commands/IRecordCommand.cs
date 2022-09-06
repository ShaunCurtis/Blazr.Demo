/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IRecordCommand<TRecord> 
    : IRequest<ValueTask<CommandResult>>
    where TRecord : class, new()
{
    public TRecord Record { get;}

    public CancellationToken CancellationToken { get; }

}
