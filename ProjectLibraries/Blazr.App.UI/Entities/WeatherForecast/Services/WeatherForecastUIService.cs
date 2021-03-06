/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public class WeatherForecastUIService
    : BaseEntityUIService<WeatherForecastEntity>
{
    public WeatherForecastUIService(ModalService modalService, NavigationManager navigationManager)
        : base(modalService, navigationManager)
    {
        this.Url = "weatherforecast";
        this.SingleTitle = "Weather Forecast";
        this.PluralTitle = "Weather Forecasts";
        this.DefaultSortField = "Date";
        this.EditForm = typeof(WeatherForecastEditForm);
        this.ViewForm = typeof(WeatherForecastViewForm);
    }
}
