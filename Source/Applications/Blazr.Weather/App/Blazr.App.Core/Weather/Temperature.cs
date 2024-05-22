/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record Temperature
{
    private readonly decimal _temperature;
    public decimal TemperatureC => _temperature;
    public decimal TemperatureF => 32 + (_temperature / 0.5556m);

    public Temperature(decimal temperatureDebCelcius)
    {
        _temperature = temperatureDebCelcius;
    }
}
