/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public interface IWeatherForecastCQSDataBroker
{
    public ValueTask<ListProviderResult<DvoWeatherForecast>> GetWeatherForecastsAsync(WeatherForecastListQuery query);

    public ValueTask<RecordProviderResult<DvoWeatherForecast>> GetWeatherForecastAsync(RecordQuery<DvoWeatherForecast> query);

    public ValueTask<CommandResult> AddWeatherForecastAsync(WeatherForecastCommand command);

    public ValueTask<CommandResult> UpdateWeatherForecastAsync(WeatherForecastCommand command);

    public ValueTask<CommandResult> DeleteWeatherForecastAsync(WeatherForecastCommand command);
}
