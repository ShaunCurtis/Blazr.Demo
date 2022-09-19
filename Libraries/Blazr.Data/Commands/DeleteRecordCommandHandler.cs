/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Data;

public sealed class DeleteRecordCommandHandler<TRecord, TDbContext>
    : IHandlerAsync<DeleteRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    private readonly IDbContextFactory<TDbContext> factory;

    public DeleteRecordCommandHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<CommandResult> ExecuteAsync(DeleteRecordCommand<TRecord> command)
    {
        using var dbContext = factory.CreateDbContext();
        dbContext.Remove<TRecord>(command.Record);
        return await dbContext.SaveChangesAsync(command.CancellationToken) == 1
            ? CommandResult.Successful("Record Deleted")
            : CommandResult.Failure("Error deleting Record");
    }
}
