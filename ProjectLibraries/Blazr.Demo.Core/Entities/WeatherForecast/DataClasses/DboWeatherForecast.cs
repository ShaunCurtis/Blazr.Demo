/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record DboWeatherForecast
{
    [Key]
    public Guid WeatherForecastId { get; init; }

    public Guid WeatherSummaryId { get; init; }

    public Guid WeatherLocationId { get; init; }

    public DateTimeOffset Date { get; init; }

    public int TemperatureC { get; init; } = 0;
}
