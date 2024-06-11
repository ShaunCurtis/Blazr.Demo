using Blazr.App.Core;
using Blazr.OneWayStreet.Core;
using Microsoft.AspNetCore.Mvc;

namespace Blazr.App.Fluent.WASM.Server;

public static class AppServices
{
    public static void AddAppAPIEndpoints(this WebApplication app)
    {
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, async ([FromBody] ListQueryAPIRequest request, IListRequestHandler<DmoWeatherForecast> handler) =>
        {
            var result = await handler.ExecuteAsync(request.ToRequest());
            return Results.Ok(result);
        });
    }
}
