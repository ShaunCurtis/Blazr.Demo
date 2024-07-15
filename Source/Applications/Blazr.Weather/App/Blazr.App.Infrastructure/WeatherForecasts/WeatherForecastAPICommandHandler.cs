/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;

namespace Blazr.App.Infrastructure;

public class WeatherForecastAPICommandHandler : ICommandHandler<DmoWeatherForecast>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastAPICommandHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<DmoWeatherForecast> request)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.Common.WeatherHttpClient);

        var apiRequest = CommandAPIRequest<DmoWeatherForecast>.FromRequest(request);

        var httpResult = await http.PostAsJsonAsync<CommandAPIRequest<DmoWeatherForecast>>(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, apiRequest, request.Cancellation);

        if (!httpResult.IsSuccessStatusCode)
            return CommandResult.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var commandAPIResult = await httpResult.Content.ReadFromJsonAsync<CommandAPIResult<Guid>>();

        CommandResult? commandResult = null;

        if (commandAPIResult is not null)
            commandResult = commandAPIResult.ToCommandResult();

        return commandResult ?? CommandResult.Failure($"No data was returned");
    }
}
