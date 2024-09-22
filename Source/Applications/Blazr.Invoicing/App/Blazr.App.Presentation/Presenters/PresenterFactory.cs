using Microsoft.AspNetCore.Components.Forms;

using System.Security.Principal;

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
        ILogger<EditPresenter<TRecord, TIdentity, TEditContext>> logger = _serviceProvider.GetRequiredService<ILogger<EditPresenter<TRecord, TIdentity, TEditContext>>>();
        var presenter = new EditPresenter<TRecord, TIdentity, TEditContext>(dataBroker, newRecordProvider, toastService, logger);
        await presenter.LoadAsync(id, isNew);

        return presenter;
    }

    public ValueTask<IListPresenter<TRecord>> CreateListPresenterAsync<TRecord>()
        where TRecord : class, new()
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();
        ILogger<ListPresenter<TRecord>> logger = _serviceProvider.GetRequiredService<ILogger<ListPresenter<TRecord>>>();
        var presenter = new ListPresenter<TRecord>(dataBroker, logger);
        return ValueTask.FromResult<IListPresenter<TRecord>>(presenter);
    }

    public async ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
        where TRecord : class, new()
        where TIdentity : IEntityKey
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();
        IToastService toastService = _serviceProvider.GetRequiredService<IToastService>();
        ILogger<ViewPresenter<TRecord, TIdentity>> logger = _serviceProvider.GetRequiredService<ILogger<ViewPresenter<TRecord, TIdentity>>>();

        var presenter = new ViewPresenter<TRecord, TIdentity>(dataBroker, logger);
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
