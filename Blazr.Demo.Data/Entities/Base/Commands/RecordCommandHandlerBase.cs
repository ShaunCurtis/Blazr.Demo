/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public abstract class RecordCommandHandlerBase<TAction, TRecord>
    : IRequestHandler<TAction, ValueTask<CommandResult>>
    where TAction : IHandlerRequest<ValueTask<CommandResult>>
    //IRecordCommand<TRecord>
{
    protected readonly IWeatherDbContext dbContext;
    protected readonly IRecordCommand<TRecord> command;

    public RecordCommandHandlerBase(IWeatherDbContext dbContext, IRecordCommand<TRecord> command)
    {
        this.command = command;
        this.dbContext = dbContext;
    }

    public async ValueTask<CommandResult> ExecuteAsync()
    {
        ExecuteCommand();
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(Guid.Empty, true, "Record Saved")
            : new CommandResult(Guid.Empty, false, "Error saving Record");
    }

    public abstract void ExecuteCommand();
}
