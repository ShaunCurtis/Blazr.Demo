/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct WeatherForecastId : IEntityKey
{
    public Guid Value { get; init; }
    public object KeyValue => this.Value;

    public WeatherForecastId(Guid value)
        => this.Value = value;

    public static WeatherForecastId NewEntity
        => new(Guid.Empty);
}

[APIInfo(pathName : "WeatherForecast", clientName: AppDictionary.Common.WeatherHttpClient)]
public sealed record DmoWeatherForecast: ICommandEntity
{
    public WeatherForecastId WeatherForecastId { get; init; } = new(Guid.Empty);
    public DateOnly Date { get; init; }
    public Temperature Temperature { get; set; } = new(0);
    public string? Summary { get; set; }
}
