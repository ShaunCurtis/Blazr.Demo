/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Data;

public class UpdateRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<UpdateRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;

    public UpdateRecordCommandHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<CommandResult> ExecuteAsync(UpdateRecordCommand<TRecord> command)
    {
        using var dbContext = factory.CreateDbContext();
        dbContext.Update<TRecord>(command.Record);
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(Guid.Empty, true, "Record Saved")
            : new CommandResult(Guid.Empty, false, "Error saving Record");
    }
}
