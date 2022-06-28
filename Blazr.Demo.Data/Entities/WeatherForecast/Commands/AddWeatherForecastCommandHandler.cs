/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class AddWeatherForecastCommandHandler
    : AddRecordCommandHandler<WeatherForecastCommand, DboWeatherForecast>
{
    public AddWeatherForecastCommandHandler(IWeatherDbContext dbContext, IRecordCommand<DboWeatherForecast> command)
        : base(dbContext,command)
    {}
}
