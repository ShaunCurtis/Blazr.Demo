/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class AddRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    public AddRecordCommand(TRecord record) : base(record)
    {}
}
