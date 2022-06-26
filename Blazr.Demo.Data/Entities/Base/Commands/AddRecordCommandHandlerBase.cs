/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public abstract class AddRecordCommandHandlerBase<TAction, TRecord>
    : RecordCommandHandlerBase<TAction, TRecord>
    where TAction : IRecordCommand<TRecord> 
{

    public AddRecordCommandHandlerBase(IWeatherDbContext dbContext, IRecordCommand<TRecord> command)
        : base(dbContext, command)
    { }

    public override void ExecuteCommand()
    {
        if (command.Record is not null)
         this.dbContext.Add(this.command.Record);
    }
}
