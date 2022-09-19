/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IRecordCommand<TRecord> 
    : IRequestAsync<ValueTask<CommandResult>>
    where TRecord : class, new()
{
    public TRecord Record { get;}
}
