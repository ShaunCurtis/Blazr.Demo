/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Text.Json.Serialization;

namespace Blazr.App.Core;

public sealed record WeatherForecastId
{
    public Guid Value { get; init; }

    [JsonConstructor]
    public WeatherForecastId(Guid value)
    {
        this.Value = value;
    }
}

public sealed record DmoWeatherForecast : ICommandEntity
{
    public WeatherForecastId WeatherForecastId { get; init; } = new(Guid.Empty);
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; set; } = new(0);
    public string? Summary { get; set; }
}
