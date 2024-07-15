/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Text.Json.Serialization;

namespace Blazr.App.Core;

public record Temperature
{
    public decimal TemperatureC { get; init; }
    public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);

    /// <summary>
    /// temperature should be provided in degrees Celcius
    /// </summary>
    /// <param name="temperature"></param>
    public Temperature(decimal temperature)
    {
        this.TemperatureC = temperature;
    }
}
