/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;

namespace Blazr.App.Infrastructure;

public class WeatherForecastAPIListRequestHandler : IListRequestHandler<DmoWeatherForecast>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastAPIListRequestHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask<ListQueryResult<DmoWeatherForecast>> ExecuteAsync(ListQueryRequest request)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.Common.WeatherHttpClient);

        var apiRequest = ListQueryAPIRequest.FromRequest(request);
        var httpResult = await http.PostAsJsonAsync<ListQueryAPIRequest>(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, apiRequest, request.Cancellation)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!httpResult.IsSuccessStatusCode)
            return ListQueryResult<DmoWeatherForecast>.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var listResult = await httpResult.Content.ReadFromJsonAsync<ListQueryResult<DmoWeatherForecast>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return listResult ?? ListQueryResult<DmoWeatherForecast>.Failure($"No data was returned");
    }
}
