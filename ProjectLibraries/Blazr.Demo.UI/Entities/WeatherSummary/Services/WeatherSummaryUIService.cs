/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class WeatherSummaryUIService
    :BaseEntityUIService<WeatherSummaryEntity>
{
    public WeatherSummaryUIService(ModalService modalService, NavigationManager navigationManager)
        : base(modalService, navigationManager)
    {
        this.SingleTitle = "Weather Summary";
        this.PluralTitle = "Weather Summaries";
        this.Url = "weathersummary";
    }
}
