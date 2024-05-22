/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Infrastructure;

/// <summary>
/// This class implements the "standard" Server Command Handler
/// against an EF `TDbContext`
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public sealed class CommandServerHandler<TDbContext>
    : ICommandHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public CommandServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<ICommandHandler<TRecord>>();

        // If one exists execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If not run the base handler
        return await this.ExecuteCommandAsync<TRecord>(request);
    }

    private async ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();

        if ((request.Item is not ICommandEntity))
            return CommandResult.Failure($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly.");

        var stateRecord = request.Item;

        // First check if it's new.
        if (request.State == CommandState.Add)
        {
            dbContext.Add<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Added")
                : CommandResult.Failure("Error adding Record");
        }

        // Check if we should delete it
        if (request.State == CommandState.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Deleted")
                : CommandResult.Failure("Error deleting Record");
        }

        // Finally it changed
        if (request.State == CommandState.Update)
        {
            dbContext.Update<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Updated")
                : CommandResult.Failure("Error saving Record");
        }

        return CommandResult.Failure("Nothing executed.  Unrecognised State.");
    }
}
