/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public class WeatherLocationUIService
    : BaseEntityUIService<WeatherLocationEntity>
{
    public WeatherLocationUIService(ModalService modalService, NavigationManager navigationManager)
        :base(modalService, navigationManager)
    {
        this.SingleTitle = "Weather Location";
        this.PluralTitle = "Weather Locations";
        this.Url = "weatherlocation";
        this.EditForm = typeof(WeatherLocationEditForm);
        this.ViewForm = typeof(WeatherLocationViewForm);
    }
}
