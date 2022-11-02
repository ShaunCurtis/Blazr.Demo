/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Core;

public record DvoWeatherForecast
    : IRecord, IAuthRecord
{
    [Key]
    public Guid Uid { get; init; }

    public Guid WeatherSummaryId { get; init; }

    public Guid WeatherLocationId { get; init; }

    public Guid OwnerId { get; init; }

    public DateTimeOffset Date { get; init; }

    public int TemperatureC { get; init; }

    public string Summary { get; init; } = String.Empty;

    public string Location { get; init; } = String.Empty;

    public string Owner { get; init; } = String.Empty;
}

