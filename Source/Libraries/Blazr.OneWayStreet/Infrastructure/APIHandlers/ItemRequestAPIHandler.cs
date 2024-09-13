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
    private readonly IHttpClientFactory _httpClientFactory;

    public ItemRequestAPIHandler(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Uses a specific handler if one is configured in DI
    /// If not, uses a generic handler using the APIInfo attributes to configure the HttpClient request  
    /// Converts the supplied KeyValue to a string and passes it as the value
    /// Note: The API endpoint needs to know how to handle it
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord, TKey>(ItemQueryRequest<TKey> request)
        where TRecord : class
    {
        IItemRequestHandler<TRecord, TKey>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<IItemRequestHandler<TRecord, TKey>>();

        // Get the custom handler
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        return await this.GetAsync<TRecord, TKey>(request);
    }

    private async ValueTask<ItemQueryResult<TRecord>> GetAsync<TRecord, TKey>(ItemQueryRequest<TKey> request)
        where TRecord : class
    {
        var attribute = Attribute.GetCustomAttribute(typeof(TRecord), typeof(APIInfo));

        if (attribute is null || !(attribute is APIInfo apiInfo))
            throw new DataPipelineException($"No API attribute defined for Record {typeof(TRecord).Name}");

        using var http = _httpClientFactory.CreateClient(apiInfo.ClientName);

        var postValue = request.Key?.ToString() ?? "No Data";
        if (request.Key is IEntityKey entityKey)
            postValue = entityKey.KeyValue.ToString();

        if (postValue is null)
            throw new DataPipelineException($"Can't convert the suppleid key value to a string for {typeof(TRecord).Name}");

        var httpResult = await http.PostAsJsonAsync<string>($"/API/{apiInfo.PathName}/GetItem", postValue, request.Cancellation)
            .ConfigureAwait(ConfigureAwaitOptions.None); 

        if (!httpResult.IsSuccessStatusCode)
            return ItemQueryResult<TRecord>.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var listResult = await httpResult.Content.ReadFromJsonAsync<ItemQueryResult<TRecord>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return listResult ?? ItemQueryResult<TRecord>.Failure($"No data was returned");
    }
}