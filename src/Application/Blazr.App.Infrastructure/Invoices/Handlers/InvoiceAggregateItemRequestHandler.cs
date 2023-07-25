/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed class InvoiceAggregateItemRequestHandler<TDbContext>
    : IItemRequestHandler<InvoiceAggregate>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public InvoiceAggregateItemRequestHandler(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<ItemQueryResult<InvoiceAggregate>> ExecuteAsync(ItemQueryRequest request)
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        Invoice? root = new();

        root = await dbContext.Set<Invoice>().SingleOrDefaultAsync(item => ((IEntity)item).Uid == request.Uid, request.Cancellation);

        if (root is null)
            return ItemQueryResult<InvoiceAggregate>.Failure("No record retrieved");

        List<InvoiceItem>? items = await dbContext.Set<InvoiceItem>()
            .Where(item => item.InvoiceUid == request.Uid)
            .ToListAsync();

        InvoiceAggregate aggregate = new(root, items);

        return ItemQueryResult<InvoiceAggregate>.Success(aggregate);
    }
}
