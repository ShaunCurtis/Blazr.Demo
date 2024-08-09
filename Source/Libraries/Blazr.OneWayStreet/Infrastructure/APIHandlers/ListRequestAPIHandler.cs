/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public sealed class ListRequestAPIHandler
    : IListRequestHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public ListRequestAPIHandler(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Uses a specific handler if one is configured in DI
    /// If not, uses a generic handler using the APIInfo attributes to configure the HttpClient request  
    /// Converts the supplied KeyValue to a string and passes it as the value
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public async ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class
    {
        IListRequestHandler<TRecord>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<IListRequestHandler<TRecord>>();

        // Get the custom handler
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        return await GetAsync<TRecord>(request);
    }

    public async ValueTask<ListQueryResult<TRecord>> GetAsync<TRecord>(ListQueryRequest request)
        where TRecord : class
    {
        var attribute = Attribute.GetCustomAttribute(typeof(TRecord), typeof(APIInfo));

        if (attribute is null || !(attribute is APIInfo apiInfo))
            throw new DataPipelineException($"No API attribute defined for Record {typeof(TRecord).Name}");

        using var http = _httpClientFactory.CreateClient(apiInfo.ClientName);
        
        var apiRequest = ListQueryAPIRequest.FromRequest(request);
        
        var httpResult = await http.PostAsJsonAsync<ListQueryAPIRequest>($"/API/{apiInfo.PathName}/GetItems", apiRequest, request.Cancellation)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!httpResult.IsSuccessStatusCode)
            return ListQueryResult<TRecord>.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var listResult = await httpResult.Content.ReadFromJsonAsync<ListQueryResult<TRecord>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return listResult ?? ListQueryResult<TRecord>.Failure($"No data was returned");
    }
}