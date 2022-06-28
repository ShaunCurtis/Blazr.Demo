/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class UpdateRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    public UpdateRecordCommand(TRecord record) : base(record)
    {}
}
