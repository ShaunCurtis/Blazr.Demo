/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class DeleteRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
    where TRecord : class, new()
{
    public DeleteRecordCommand(TRecord record) : base(record)
    {}
}
