/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Infrastructure;

public sealed class CustomerRequestServerHandler<TDbContext>
    : IItemRequestHandler<Customer>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ItemRequestServerHandler<TDbContext> _baseHandler;

    public CustomerRequestServerHandler(IServiceProvider serviceProvider, ItemRequestServerHandler<TDbContext> serverHandler)
    {
        _serviceProvider = serviceProvider;
        _baseHandler = serverHandler;
    }

    public async ValueTask<ItemQueryResult<Customer>> ExecuteAsync(ItemQueryRequest request)
    {
        return await _baseHandler.ExecuteAsync<Customer>(request);
    }
}
