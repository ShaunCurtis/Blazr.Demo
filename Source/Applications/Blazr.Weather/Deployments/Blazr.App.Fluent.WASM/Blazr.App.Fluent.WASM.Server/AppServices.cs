using Blazr.App.Core;
using Blazr.OneWayStreet.Core;
using Microsoft.AspNetCore.Mvc;

namespace Blazr.App.Fluent.WASM.Server;

public static class AppServices
{
    public static void AddAppAPIEndpoints(this WebApplication app)
    {
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, async ([FromBody] ListQueryAPIRequest request, IListRequestHandler<DmoWeatherForecast> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.ExecuteAsync(request.ToRequest(cancellationToken));
            return Results.Ok(result);
        });

        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastItemAPIUrl, async ([FromBody] ItemQueryAPIRequest<Guid> request, IItemRequestHandler<DmoWeatherForecast, Guid> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.ExecuteAsync(request.ToRequest(cancellationToken));
            return Results.Ok(result);
        });
    }
}
