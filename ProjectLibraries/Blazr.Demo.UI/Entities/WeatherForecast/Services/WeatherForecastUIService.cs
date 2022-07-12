/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public class WeatherForecastUIService : IEntityUIService<WeatherForecastEntity>
{
    public string Url => "weatherforecast";

    public string SingleTitle => "Weather Forecast";

    public string PluralTitle => "Weather Forecasts";

    public Type? EditForm => typeof(WeatherForecastEditForm);

    public Type? ViewForm => typeof(WeatherForecastViewForm);
}
