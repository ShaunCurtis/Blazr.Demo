/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Text.Json.Serialization;

namespace Blazr.App.Core;

public readonly record struct Temperature
{
    public decimal TemperatureC { get; init; } = -273;
    public bool IsValid { get; init; }
    [JsonIgnore] public decimal TemperatureF => 32 + (this.TemperatureC / 0.5556m);

    public Temperature() { }

    /// <summary>
    /// temperature should be provided in degrees Celcius
    /// </summary>
    /// <param name="temperature"></param>
    public Temperature(decimal temperatureAsDegCelcius)
    {
        this.TemperatureC = temperatureAsDegCelcius;
        if (temperatureAsDegCelcius > -273)
            IsValid = true;
    }

    public override string ToString()
    {
        return this.IsValid ? TemperatureC.ToString() : "Not Valid";
    }
}
