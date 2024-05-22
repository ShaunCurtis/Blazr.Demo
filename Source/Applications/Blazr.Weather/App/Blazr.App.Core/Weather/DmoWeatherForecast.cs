/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record WeatherForecastId(Guid Value) : IGuidKey;

public record DmoWeatherForecast : ICommandEntity
{
    public WeatherForecastId WeatherForecastId { get; init; } = new(Guid.Empty);
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; set; } = new(0);
    public string? Summary { get; set; }
}
