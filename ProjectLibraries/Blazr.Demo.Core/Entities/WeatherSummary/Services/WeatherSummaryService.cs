/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class WeatherSummaryService
    :BaseEntityService<WeatherSummaryEntity>
{
    public WeatherSummaryService()
    {
        this.SingleTitle = "Weather Summary";
        this.PluralTitle = "Weather Summaries";
        this.Url = "weathersummary";
    }
}
