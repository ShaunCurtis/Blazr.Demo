/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.App.Core;

public class Temperature
{
    private decimal _temperature;

    public decimal Celcius => _temperature;

    public decimal Centigrade => _temperature;

    public decimal Kelvin => _temperature - 272.15m;

    public decimal Fahrenheit => ((_temperature * 9) / 5) + 32;

    public Temperature(decimal temperature, TemperatureUnits unit)
    {
        decimal value = unit switch
        {
            TemperatureUnits.Fahrenheit => _temperature = ((temperature * 5) / 9) - 32,
            TemperatureUnits.Kelvin => _temperature + 272.15m,
            _ => _temperature = temperature,
        };
        if (value < -272.15m)
            throw new TemperatureValueException(value);
    }

    public decimal GetTemperature(TemperatureUnits units)
        => units switch
        {
            TemperatureUnits.Fahrenheit => this.Fahrenheit,
            TemperatureUnits.Kelvin => this.Kelvin,
            _ => this._temperature
        };

    public enum TemperatureUnits { Fahrenheit, Celcius, Centigrade, Kelvin }
}

public class TemperatureValueException : Exception
{
    public TemperatureValueException(decimal value)
        : base($"{value} is not a valid Temperature") { }
}
