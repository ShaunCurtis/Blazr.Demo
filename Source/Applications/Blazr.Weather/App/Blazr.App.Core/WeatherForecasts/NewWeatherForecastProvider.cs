/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using UuidExtensions;

namespace Blazr.App.Core;

public class NewWeatherForecastProvider : INewRecordProvider<DmoWeatherForecast>
{
    public DmoWeatherForecast NewRecord()
    {
        return new DmoWeatherForecast() { 
            Id = new(Uuid7.Guid()), 
            Date = new(DateTime.Now) 
        };
    }
}

