/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class PresenterFactory
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
        IToastService toastService = _serviceProvider.GetRequiredService<IToastService>();

        var presenter = new EditPresenter<TRecord, TIdentity, TEditContext>(dataBroker, newRecordProvider, toastService);
        await presenter.LoadAsync(id, isNew);

        return presenter;
    }

    public ValueTask<IListPresenter<TRecord>> CreateListPresenterAsync<TRecord>()
    where TRecord : class, new()
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();

        var presenter = new ListPresenter<TRecord>(dataBroker);

        return ValueTask.FromResult<IListPresenter<TRecord>>(presenter);
    }

    public async ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
    where TRecord : class, new()
    where TIdentity : IEntityKey
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();
        IToastService toastService = _serviceProvider.GetRequiredService<IToastService>();

        var presenter = new ViewPresenter<TRecord, TIdentity>(dataBroker);
        await presenter.LoadAsync(id);

        return presenter;
    }


}
