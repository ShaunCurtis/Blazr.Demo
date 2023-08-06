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
    private readonly ILogger<InvoiceAggregateCommandHandler<TDbContext>> _logger;
    private readonly IDboEntityMap<DboInvoice, Invoice> _invoiceMap;
    private readonly IDboEntityMap<DboInvoiceItem, InvoiceItem> _invoiceItemMap;

    public InvoiceAggregateCommandHandler(IDbContextFactory<TDbContext> factory, ILogger<InvoiceAggregateCommandHandler<TDbContext>> logger, IDboEntityMap<DboInvoice, Invoice> invoiceMap, IDboEntityMap<DboInvoiceItem, InvoiceItem> invoiceItemMap)
    {
        _factory = factory;
        _logger = logger;
        _invoiceMap = invoiceMap;
        _invoiceItemMap = invoiceItemMap;
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<InvoiceAggregate> request)
    {
        using var dbContext = _factory.CreateDbContext();

        var aggregate = request.Item;
        var root = aggregate.Root;
        var dboRoot = _invoiceMap.Map(root);

        // Create the aggregate if it's marked as new
        if (root.EntityState.IsNew)
            dbContext.Add<DboInvoice>(dboRoot);

        // Delete the aggregate if it is maarked as deleted)
        else if (root.EntityState.MarkedForDeletion)
            dbContext.Remove<DboInvoice>(dboRoot);

        // Update the aggregate if it is marked as modified)
        else if (aggregate.IsRootDirty)
            dbContext.Update<DboInvoice>(dboRoot);

        // Update all the existing items based on their state
        foreach (var item in aggregate.AllItems)
        {
            var dboItem = _invoiceItemMap.Map(item);

            if (item.EntityState.IsNew)
                dbContext.Add<DboInvoiceItem>(dboItem);

            else if (item.EntityState.MarkedForDeletion)
                dbContext.Remove<DboInvoiceItem>(dboItem);

            else if (item.EntityState.IsMutated)
                dbContext.Update<DboInvoiceItem>(dboItem);
        }

        try
        {
            // Commit all changes as a single transaction
            var transactions = await dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateException)
        {
            var message = $"Failed to save the Invoice aggregate {request.Item.Uid}.  Transaction aborted";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }
        catch (Exception e)
        {
            var message = $"An error occured trying to save the Invoice aggregate {request.Item.Uid}.  Detail: {e.Message}.";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }
    }
}
