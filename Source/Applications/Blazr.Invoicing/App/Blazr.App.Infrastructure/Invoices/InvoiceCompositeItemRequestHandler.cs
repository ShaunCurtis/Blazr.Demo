/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

/// <summary>
/// Class to build an Invoice Composite from the data store
/// This requires getting the invoice and the associated invoice items
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public sealed class InvoiceCompositeItemRequestHandler<TDbContext>
    : IItemRequestHandler<InvoiceComposite, InvoiceId>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;
    private readonly IServiceProvider _serviceProvider;

    public InvoiceCompositeItemRequestHandler(IDbContextFactory<TDbContext> factory, IServiceProvider serviceProvider)
    {
        _factory = factory;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<ItemQueryResult<InvoiceComposite>> ExecuteAsync(ItemQueryRequest<InvoiceId> request)
    {
        // Get a DbContext scoped to the method and turn off change tracking 
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Get the Invoice  record
        var invoiceid = request.Key.Value;
        var dboRoot = await dbContext.Set<DvoInvoice>()
            .SingleOrDefaultAsync(item => item.InvoiceID == invoiceid, request.Cancellation);

        // If we don't get anything return failure
        if (dboRoot is null)
            return ItemQueryResult<InvoiceComposite>.Failure("No record retrieved");

        // Map the data store object to the domain entity
        var root = DvoInvoiceMap.Map(dboRoot);

        // Get all the invoice items.  it may be none
        List<DmoInvoiceItem> items = await dbContext.Set<DboInvoiceItem>()
            .Where(item => item.InvoiceID == invoiceid)
            .Select(item => DboInvoiceItemMap.Map(item))
            .ToListAsync();

        // create the composite with the data store retrieved items
        var composite = new InvoiceComposite(_serviceProvider, root, items);

        // Return success
        return ItemQueryResult<InvoiceComposite>.Success(composite);
    }
}