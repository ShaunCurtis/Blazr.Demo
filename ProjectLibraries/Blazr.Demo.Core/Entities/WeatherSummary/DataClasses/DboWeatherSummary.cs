/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Core;

public record DboWeatherSummary
{
    [Key]
    public Guid WeatherSummaryId { get; init; } = Guid.Empty;

    public string Summary { get; init; } = string.Empty;
}
