/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Linq.Expressions;

namespace Blazr.App.Infrastructure;

public class WeatherForecastFilterBySummarySpecification : PredicateSpecification<DboWeatherForecast>
{
    private string? _summary;

    public WeatherForecastFilterBySummarySpecification()
    { }

    public WeatherForecastFilterBySummarySpecification(FilterDefinition filter)
    {
        _summary = filter.FilterData;
    }

    public override Expression<Func<DboWeatherForecast, bool>> Expression
        => item => item.Summary != null ? item.Summary.Equals(_summary) : false;
}
