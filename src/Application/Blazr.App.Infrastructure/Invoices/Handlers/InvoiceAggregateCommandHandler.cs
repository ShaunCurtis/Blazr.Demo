/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed class InvoiceAggregateCommandHandler<TDbContext>
    : ICommandHandler<InvoiceAggregate>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;
    private ILogger<InvoiceAggregateCommandHandler<TDbContext>> _logger;

    public InvoiceAggregateCommandHandler(IDbContextFactory<TDbContext> factory, ILogger<InvoiceAggregateCommandHandler<TDbContext>> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<InvoiceAggregate> request)
    {
        using var dbContext = _factory.CreateDbContext();

        var aggregate = request.Item;
        var root = aggregate.Root;
        var dboRoot = root.ToDbo();

        // Create the aggregate if it's marked as new
        if (root.StateCode == StateCodes.New)
            dbContext.Add<DboInvoice>(dboRoot);

        // Delete the aggregate if it is maarked as deleted)
        if (root.StateCode == StateCodes.Delete)
            dbContext.Remove<DboInvoice>(dboRoot);

        // Update the aggregate if it is marked as modified)
        if (aggregate.IsRootDirty)
            dbContext.Update<DboInvoice>(dboRoot);

        // Update all the existing items based on their state
        foreach (var item in aggregate.AllItems)
        {
            var dboItem = item.ToDbo();

            if (item.StateCode == StateCodes.New)
                dbContext.Add<DboInvoiceItem>(dboItem);

            if (StateCodes.IsUpdate(item.StateCode) && aggregate.IsCollectionItemDirty(item))
                dbContext.Update<DboInvoiceItem>(dboItem);

            if (item.StateCode == StateCodes.Delete)
                dbContext.Remove<DboInvoiceItem>(dboItem);
        }

        try
        {
            // Commit all changes as a single transaction
            var transactions = await dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateException)
        {
            var message = $"Failed to save the ToDo aggregate {request.Item.Uid}.  Transaction aborted";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }
        catch (Exception e)
        {
            var message = $"An error occured trying to save ToDo aggregate {request.Item.Uid}.  Detail: {e.Message}.";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }
    }
}
