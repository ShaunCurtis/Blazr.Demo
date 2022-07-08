/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Core;

public record DboWeatherLocation
{
    [Key]
    public Guid WeatherLocationId { get; init; }

    public string Location { get; init; } = String.Empty;
}
