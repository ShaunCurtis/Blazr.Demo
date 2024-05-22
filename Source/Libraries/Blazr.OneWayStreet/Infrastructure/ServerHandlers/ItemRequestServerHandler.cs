/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Infrastructure;

public sealed class ItemRequestServerHandler<TDbContext>
    : IItemRequestHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public ItemRequestServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<IItemRequestHandler<TRecord>>();

        // If we one is registered in DI and execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If not run the base handler
        return await this.GetItemAsync<TRecord>(request);
    }

    private async ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request)
    where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var record = await dbContext.Set<TRecord>().FindAsync(request.KeyValue, request.Cancellation);

       // var record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => item.EntityUid == request.Uid, request.Cancellation);

        if (record is null)
            return ItemQueryResult<TRecord>.Failure($"No record retrieved with a Uid of {request.KeyValue.ToString()}");

        return ItemQueryResult<TRecord>.Success(record);
    }
}
