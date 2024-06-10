/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;

namespace Blazr.App.Infrastructure;

public class WeatherAPIListRequestHandler : IListRequestHandler<DmoWeatherForecast>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherAPIListRequestHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask<ListQueryResult<DmoWeatherForecast>> ExecuteAsync(ListQueryRequest request)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.WeatherForecast.WeatherHttpClient);

        var httpResult = await http.PostAsJsonAsync<ListQueryRequest>(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, request);

        if (!httpResult.IsSuccessStatusCode)
            return ListQueryResult<DmoWeatherForecast>.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var listResult = await httpResult.Content.ReadFromJsonAsync<ListQueryResult<DmoWeatherForecast>>();

        return listResult ?? ListQueryResult<DmoWeatherForecast>.Failure($"No data was returned");
    }
}
