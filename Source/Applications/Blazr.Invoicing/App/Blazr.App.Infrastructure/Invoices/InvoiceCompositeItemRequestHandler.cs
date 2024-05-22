/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed class InvoiceCompositeItemRequestHandler<TDbContext>
    : IItemRequestHandler<InvoiceComposite>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public InvoiceCompositeItemRequestHandler(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<ItemQueryResult<InvoiceComposite>> ExecuteAsync(ItemQueryRequest request)
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        if (request.KeyValue is not Guid invoiceid)
            return ItemQueryResult<InvoiceComposite>.Failure("KeyValue in the Request is not a Guid");

        var dboRoot = await dbContext.Set<DvoInvoice>()
            .SingleOrDefaultAsync(item => item.InvoiceID == invoiceid, request.Cancellation);

        if (dboRoot is null)
            return ItemQueryResult<InvoiceComposite>.Failure("No record retrieved");

        var root = DvoInvoiceMap.Map(dboRoot);

        var sections = await dbContext.Set<DboInvoiceItem>()
            .Where(item => item.InvoiceID == invoiceid)
            .Select(item => DboInvoiceItemMap.Map(item))
            .ToListAsync();

        var composite = new InvoiceComposite(root, sections ?? Enumerable.Empty<DmoInvoiceItem>());

        return ItemQueryResult<InvoiceComposite>.Success(composite);
    }
}