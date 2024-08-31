/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class EditPresenterFactory
{
    private IServiceProvider _serviceProvider

    public EditPresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IEditPresenter<TRecord, TIdentity, TEditContext>> CreateAsync<TRecord, TIdentity, TEditContext>(TIdentity id, bool isNew)
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
}
