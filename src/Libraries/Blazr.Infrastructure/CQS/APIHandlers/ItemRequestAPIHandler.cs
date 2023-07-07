/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Infrastructure;

public sealed class ItemRequestAPIHandler
    : IItemRequestHandler
{
    private IHttpClientFactory _factory;
    private readonly IServiceProvider _serviceProvider;

    public ItemRequestAPIHandler(IServiceProvider serviceProvider, IHttpClientFactory factory)
    { 
        _factory = factory;
        _serviceProvider = serviceProvider;
    }
    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
    where TRecord : class, IGuidIdentity, new()
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<IItemRequestHandler<TRecord>>();

        // If we get one then one is registered in DI and we execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If there's no custom handler registered we run the default handler
        return await this.GetItemAsync<TRecord>(request);
    }

    private async ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, IGuidIdentity, new()
    {
        var entityname = (new TRecord()).GetType().Name;

        var httpClient = _factory.CreateClient();
        //Add security here
        var response = await httpClient.PostAsJsonAsync<ItemQueryRequest>($"/api/{entityname}/listquery", request, request.Cancellation);

        ItemQueryResult<TRecord>? result = null;

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ItemQueryResult<TRecord>>();

        return result ?? ItemQueryResult<TRecord>.Failure($"{response.StatusCode} = {response.ReasonPhrase}"); ;
    }
}
