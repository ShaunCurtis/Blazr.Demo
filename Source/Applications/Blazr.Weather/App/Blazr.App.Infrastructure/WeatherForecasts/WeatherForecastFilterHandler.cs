/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class WeatherForecastFilterHandler : RecordFilterHandler<DboWeatherForecast>, IRecordFilterHandler<DboWeatherForecast>
{
    public override IPredicateSpecification<DboWeatherForecast>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            AppDictionary.WeatherForecast.WeatherForecastFilterBySummarySpecification => new WeatherForecastFilterBySummarySpecification(filter),
            _ => null
        };
}
