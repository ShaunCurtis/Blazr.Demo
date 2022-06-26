/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public abstract class DeleteRecordCommandHandlerBase<TAction, TRecord>
    : RecordCommandHandlerBase<TAction, TRecord>
    where TAction : IRecordCommand<TRecord>
{

    public DeleteRecordCommandHandlerBase(IWeatherDbContext dbContext, IRecordCommand<TRecord> command)
        : base(dbContext, command)
    { }

    public override void ExecuteCommand()
    {
        if (command.Record is not null)
         this.dbContext.Remove(this.command.Record);
    }
}
