/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public record DvoWeatherForecast 
{
    [Key]
    public Guid WeatherForecastId { get; init; }

    public Guid WeatherSummaryId { get; init; }

    public DateTimeOffset Date { get; init; }

    public int TemperatureC { get; init; }

    public string? Summary { get; init; }
}

