/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public sealed class ItemRequestAPIHandler
    : IItemRequestHandler
{
    private readonly IServiceProvider _serviceProvider;

    public ItemRequestAPIHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class
    {
        IItemRequestHandler<TRecord>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<IItemRequestHandler<TRecord>>();

        // Get the custom handler
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If there's no custom handler throw an exception
        throw new DataPipelineException("No API Handler defined for this action");
    }
}