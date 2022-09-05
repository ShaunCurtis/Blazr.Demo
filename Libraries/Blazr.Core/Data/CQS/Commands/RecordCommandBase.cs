/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public abstract class RecordCommandBase<TRecord>
     : IRecordCommand<TRecord>
    where TRecord : class, new()
{
    public Guid TransactionId { get; } = Guid.NewGuid(); 
    
    public TRecord Record { get; protected set; } = default!;

    public CancellationToken CancellationToken { get; } = new CancellationToken();

    public RecordCommandBase(TRecord record)
        => this.Record = record;
}
