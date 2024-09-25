/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Presentation.Toasts;

namespace Blazr.App.Presentation.Mud;

public class MudPresenterFactory
{
    private readonly IDataBroker _dataBroker;
    private readonly IAppToastService _toastService;
    private readonly INewRecordProvider<DmoWeatherForecast> _newProvider;
    private readonly IPresenterFactory _presenterFactory;

    public MudPresenterFactory(IDataBroker dataBroker, IPresenterFactory presenterFactory, IAppToastService toastService, INewRecordProvider<DmoWeatherForecast> newProvider)
    {
        _dataBroker = dataBroker;
        _toastService = toastService;
        _newProvider = newProvider;
        _presenterFactory = presenterFactory;
    }

    public ValueTask<IMudGridPresenter> CreateDataGridPresenterAsync()
    {
        var presenter = new MudGridPresenter(_dataBroker);

        return ValueTask.FromResult((IMudGridPresenter)presenter);
    }

    public ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
        where TRecord : class, new()
    {
        return _presenterFactory.CreateViewPresenterAsync<TRecord, TIdentity>(id);
    }

    public ValueTask<IEditPresenter<TRecord, TIdentity, TEditContext>> CreateEditPresenterAsync<TRecord, TIdentity, TEditContext>(TIdentity id, bool isNew)
        where TRecord : class, new()
        where TEditContext : IRecordEditContext<TRecord>, new()
    {
        return _presenterFactory.CreateEditPresenterAsync<TRecord, TIdentity, TEditContext>(id, isNew);
    }

    public ValueTask<GuidLookUpPresenter<TLookupItem>> CreateGuidLookupPresenterAsync<TLookupItem>()
        where TLookupItem : class, IGuidLookUpItem, new()
    {
        return _presenterFactory.CreateGuidLookupPresenterAsync<TLookupItem>();
    }
}
