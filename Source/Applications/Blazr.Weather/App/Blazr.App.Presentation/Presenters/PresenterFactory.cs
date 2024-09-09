/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class PresenterFactory : IPresenterFactory
{
    private IServiceProvider _serviceProvider;

    public PresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IEditPresenter<TRecord, TIdentity, TEditContext>> CreateEditPresenterAsync<TRecord, TIdentity, TEditContext>(TIdentity id, bool isNew)
        where TRecord : class, new()
        where TIdentity : IEntityKey
        where TEditContext : IRecordEditContext<TRecord>, new()
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();
        ICommandHandler<TRecord> commandHandler = _serviceProvider.GetRequiredService<ICommandHandler<TRecord>>();
        INewRecordProvider<TRecord> newRecordProvider = _serviceProvider.GetRequiredService<INewRecordProvider<TRecord>>();
        IAppToastService toastService = _serviceProvider.GetRequiredService<IAppToastService>();

        var presenter = new EditPresenter<TRecord, TIdentity, TEditContext>(dataBroker, newRecordProvider, toastService);
        await presenter.LoadAsync(id, isNew);

        return presenter;
    }

    public ValueTask<IDataGridPresenter> CreateDataGridPresenterAsync()
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();

        var presenter = new DataGridPresenter(dataBroker);

        return ValueTask.FromResult<IDataGridPresenter>(presenter);
    }

    public async ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
        where TRecord : class, new()
        where TIdentity : IEntityKey
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();
        IAppToastService toastService = _serviceProvider.GetRequiredService<IAppToastService>();

        var presenter = new ViewPresenter<TRecord, TIdentity>(dataBroker);
        await presenter.LoadAsync(id);

        return presenter;
    }

    public async ValueTask<GuidLookUpPresenter<TLookupItem>> CreateGuidLookupPresenterAsync<TLookupItem>()
        where TLookupItem : class, IGuidLookUpItem, new()
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();

        var presenter = new GuidLookUpPresenter<TLookupItem>(dataBroker);
        await presenter.LoadAsync();

        return presenter;
    }
}
