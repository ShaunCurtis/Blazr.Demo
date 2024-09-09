/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation.FluentUI;
using Blazr.Presentation.Toasts;

namespace Blazr.App.Presentation;

public class FluentUIPresenterFactory
{
    private readonly IDataBroker _dataBroker;
    private readonly IAppToastService _toastService;
    private readonly INewRecordProvider<DmoWeatherForecast> _newProvider;
    private readonly IPresenterFactory _presenterFactory;

    public FluentUIPresenterFactory(IDataBroker dataBroker, IPresenterFactory presenterFactory, IAppToastService toastService, INewRecordProvider<DmoWeatherForecast> newProvider)
    {
        _dataBroker = dataBroker;
        _toastService = toastService;
        _newProvider = newProvider;
        _presenterFactory = presenterFactory;
    }

    public ValueTask<IFluentGridPresenter> CreateDataGridPresenterAsync()
    {
        var presenter = new FluentGridPresenter(_dataBroker);

        return ValueTask.FromResult((IFluentGridPresenter)presenter);
    }

    public ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
        where TRecord : class, new()
        where TIdentity : IEntityKey
    {
        return _presenterFactory.CreateViewPresenterAsync<TRecord, TIdentity>(id);
    }

    public ValueTask<IEditPresenter<TRecord, TIdentity, TEditContext>> CreateEditPresenterAsync<TRecord, TIdentity, TEditContext>(TIdentity id, bool isNew)
        where TRecord : class, new()
        where TIdentity : IEntityKey
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
