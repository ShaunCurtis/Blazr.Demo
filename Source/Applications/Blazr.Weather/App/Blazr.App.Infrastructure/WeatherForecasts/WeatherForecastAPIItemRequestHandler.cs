/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;

namespace Blazr.App.Infrastructure;

public class WeatherForecastAPIItemRequestHandler : IItemRequestHandler<DmoWeatherForecast, WeatherForecastId>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastAPIItemRequestHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask<ItemQueryResult<DmoWeatherForecast>> ExecuteAsync(ItemQueryRequest<WeatherForecastId> request)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.WeatherForecast.WeatherHttpClient);

        var apiRequest = new ItemQueryAPIRequest<Guid>(request.Key.Value);
        var httpResult = await http.PostAsJsonAsync<ItemQueryAPIRequest<Guid>>(AppDictionary.WeatherForecast.WeatherForecastItemAPIUrl, apiRequest, request.Cancellation);

        if (!httpResult.IsSuccessStatusCode)
            return ItemQueryResult<DmoWeatherForecast>.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var listResult = await httpResult.Content.ReadFromJsonAsync<ItemQueryResult<DmoWeatherForecast>>();

        return listResult ?? ItemQueryResult<DmoWeatherForecast>.Failure($"No data was returned");
    }
}
