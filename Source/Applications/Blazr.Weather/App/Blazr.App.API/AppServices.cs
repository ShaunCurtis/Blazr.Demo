using Blazr.App.Core;
using Blazr.OneWayStreet.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Blazr.App.API;

public static class AppAPIServices
{
    public static void AddAppAPIEndpoints(this WebApplication app)
    {
        AddGenericWeatherForecastAPIEndpoints(app);
    }

    public static void AddWeatherForecastAPIEndpoints(this WebApplication app)
    {
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, async ([FromBody] ListQueryAPIRequest request, IListRequestHandler<DmoWeatherForecast> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.ExecuteAsync(request.ToRequest(cancellationToken));
            return Results.Ok(result);
        });

        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastItemAPIUrl, async ([FromBody] ItemQueryAPIRequest<Guid> apiRequest, IItemRequestHandler<DmoWeatherForecast, WeatherForecastId> handler, CancellationToken cancellationToken) =>
        {
            var request = new ItemQueryRequest<WeatherForecastId>(new(apiRequest.KeyValue), cancellationToken);
            var result = await handler.ExecuteAsync(request);
            return Results.Ok(result);
        });

        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, async ([FromBody] CommandAPIRequest<DmoWeatherForecast> request, ICommandHandler<DmoWeatherForecast> handler, CancellationToken cancellationToken) =>
        {
            var commandResult = await handler.ExecuteAsync(request.ToRequest(cancellationToken));
            CommandAPIResult<Guid> result = new();
            Guid key = Guid.Empty;

            // See if we have a returned Guid key
            Guid.TryParse(commandResult.KeyValue?.ToString(), out key);

            result = new CommandAPIResult<Guid>() { Successful = commandResult.Successful, Message=commandResult.Message, KeyValue = key };

            return Results.Ok(result);
        });
    }

    public static void AddGenericWeatherForecastAPIEndpoints(this WebApplication app)
    {
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, async ([FromBody] ListQueryAPIRequest request, IListRequestHandler<DmoWeatherForecast> handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.ExecuteAsync(request.ToRequest(cancellationToken));
            return Results.Ok(result);
        });

        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastItemAPIUrl, async ([FromBody] string apiRequest, IItemRequestHandler<DmoWeatherForecast, WeatherForecastId> handler, CancellationToken cancellationToken) =>
        {
            if (!Guid.TryParse(apiRequest, out var id))
                return Results.NoContent();

            var request = new ItemQueryRequest<WeatherForecastId>(new(id), cancellationToken);
            var result = await handler.ExecuteAsync(request);
            return Results.Ok(result);
        });

        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, async ([FromBody] CommandAPIRequest<DmoWeatherForecast> request, ICommandHandler<DmoWeatherForecast> handler, CancellationToken cancellationToken) =>
        {
            var commandResult = await handler.ExecuteAsync(request.ToRequest(cancellationToken));
            CommandAPIResult<Guid> result = new();
            Guid key = Guid.Empty;

            // See if we have a returned Guid key
            Guid.TryParse(commandResult.KeyValue?.ToString(), out key);

            result = new CommandAPIResult<Guid>() { Successful = commandResult.Successful, Message = commandResult.Message, KeyValue = key };

            return Results.Ok(result);
        });
    }
}
