/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Data;

public sealed class UpdateRecordCommandHandler<TRecord, TDbContext>
    : IHandlerAsync<UpdateRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    private readonly IDbContextFactory<TDbContext> factory;

    public UpdateRecordCommandHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<CommandResult> ExecuteAsync(UpdateRecordCommand<TRecord> command)
    {
        using var dbContext = factory.CreateDbContext();
        dbContext.Update<TRecord>(command.Record);
        return await dbContext.SaveChangesAsync(command.CancellationToken) == 1
            ? CommandResult.Successful("Record Updated")
            : CommandResult.Failure("Error updating Record");
    }
}
