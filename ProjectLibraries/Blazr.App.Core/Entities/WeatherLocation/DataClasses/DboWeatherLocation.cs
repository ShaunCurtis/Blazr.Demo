/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record DboWeatherLocation
    : IAuthRecord
{
    [Key]
    public Guid WeatherLocationId { get; init; }

    public Guid OwnerId { get; init; }

    public string Location { get; init; } = String.Empty;
}
