/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public class WeatherLocationService
    : BaseEntityService<WeatherLocationEntity>
{
    public WeatherLocationService()
    {
        this.SingleTitle = "Weather Location";
        this.PluralTitle = "Weather Locations";
        this.Url = "weathersummary";
    }
}
