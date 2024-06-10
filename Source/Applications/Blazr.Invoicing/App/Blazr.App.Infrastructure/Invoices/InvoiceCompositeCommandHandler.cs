﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed class InvoiceCompositeCommandHandler<TDbContext>
    : ICommandHandler<InvoiceComposite>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;
    private readonly ILogger<InvoiceCompositeCommandHandler<TDbContext>> _logger;

    public InvoiceCompositeCommandHandler(IDbContextFactory<TDbContext> factory, ILogger<InvoiceCompositeCommandHandler<TDbContext>> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<InvoiceComposite> request)
    {
        using var dbContext = _factory.CreateDbContext();

        var composite = request.Item;
        var dboRoot = DboInvoiceMap.Map(composite.Invoice);

        var rootState = composite.State; 

        // Update the root item based on its state
        if (rootState == FluxState.New)
            dbContext.Add<DboInvoice>(dboRoot);

        else if (rootState == FluxState.Deleted)
            dbContext.Remove<DboInvoice>(dboRoot);

        else if (rootState == FluxState.Modified)
            dbContext.Update<DboInvoice>(dboRoot);

        // Update all the existing items based on their state
        foreach (var item in composite.InvoiceItems)
        {
            var dboItem = DboInvoiceItemMap.Map(item);

            // If the root state is delete then we delete everything regardless of item state
            var itemState = rootState;
            if (rootState != FluxState.Deleted)
                itemState = composite.GetInvoiceItemState(item.Id);

            if (itemState == FluxState.New)
                dbContext.Add<DboInvoiceItem>(dboItem);

            else if (itemState == FluxState.Deleted)
                dbContext.Remove<DboInvoiceItem>(dboItem);

            else if (itemState == FluxState.Modified)
                dbContext.Update<DboInvoiceItem>(dboItem);
        }

        try
        {
            // Commit all changes as a single transaction
            var transactions = await dbContext.SaveChangesAsync();
            composite.Persisted();
            return CommandResult.Success();
        }

        catch (DbUpdateException)
        {
            var message = $"Failed to save the composite {request.Item.Invoice.InvoiceId.Value}.  Transaction aborted";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }

        catch (Exception e)
        {
            var message = $"An error occurred trying to save composite {request.Item.Invoice.InvoiceId.Value}.  Detail: {e.Message}.";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }
    }
}