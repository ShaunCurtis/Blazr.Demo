/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Text.Json.Serialization;

namespace Blazr.App.Core;

public record Temperature
{
    private readonly decimal temperature;
    public decimal TemperatureC => temperature;
    public decimal TemperatureF => 32 + (temperature / 0.5556m);
    
    public Temperature() { }

    public Temperature(decimal temperature)
    {
        this.temperature = temperature;
    }
}
