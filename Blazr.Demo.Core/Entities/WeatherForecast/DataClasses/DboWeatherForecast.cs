/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Core;

public record DboWeatherForecast 
{
    [Key]
    public Guid WeatherForecastId { get; init; } = Guid.Empty;

    public Guid WeatherSummaryId { get; init; } = Guid.Empty;

    public DateTimeOffset Date { get; init; }

    public int TemperatureC { get; init; }

}
