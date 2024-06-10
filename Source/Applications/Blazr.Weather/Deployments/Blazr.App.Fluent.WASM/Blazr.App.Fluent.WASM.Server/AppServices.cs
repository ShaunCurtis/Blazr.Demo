using Blazr.App.Core;
using Blazr.OneWayStreet.Core;
using Microsoft.AspNetCore.Mvc;

namespace Blazr.App.Fluent.WASM.Server;

public static class AppServices
{
    public static void AddAppAPIEndpoints(this WebApplication app)
    {
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, async ([FromBody] ListQueryRequest request, IListRequestHandler<DboWeatherForecast> handler) =>
        {
            var result = await handler.ExecuteAsync(request);
            return Results.Ok(result);
        });
    }
}
