/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Presentation.Toasts;

namespace Blazr.App.Presentation;

public class WeatherForecastPresenterFactory
{
    private readonly IDataBroker _dataBroker;
    private readonly IAppToastService _toastService;
    private readonly INewRecordProvider<DmoWeatherForecast> _newProvider;

    public WeatherForecastPresenterFactory(IDataBroker dataBroker, IAppToastService toastService, INewRecordProvider<DmoWeatherForecast> newProvider)
    {
        _dataBroker = dataBroker;
        _toastService = toastService;
        _newProvider = newProvider;
    }

    public async ValueTask<WeatherForecastEditPresenter> GetEditPresenterAsync(WeatherForecastId id)
    {
        var presenter = new WeatherForecastEditPresenter(_dataBroker, _toastService, _newProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }

    public async ValueTask<IViewPresenter<DmoWeatherForecast, WeatherForecastId>> GetViewPresenterAsync(WeatherForecastId id)
    {
        var presenter = new ViewPresenter<DmoWeatherForecast, WeatherForecastId>(_dataBroker);
        await presenter.LoadAsync(id);
        return presenter;
    }

}
