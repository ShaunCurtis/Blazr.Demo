/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class BaseForeignKeyService<TFkRecord, TService>
    : IForeignKeyService<TFkRecord, TService>, IDisposable
        where TFkRecord : class, IFkListItem, new()
        where TService : class, IEntityService
{
    private IEnumerable<TFkRecord> _fkList = Enumerable.Empty<TFkRecord>();
    protected INotificationService<TService> NotificationService;
    protected IDataBroker DataBroker;
    private bool _firstLoad = true;

    public IEnumerable<TFkRecord> FkList => _fkList;

    public IEnumerable<IFkListItem> List => new List<IFkListItem>(_fkList.Cast<IFkListItem>());

    public BaseForeignKeyService(IDataBroker dataBroker, INotificationService<TService> notificationService)
    {
        NotificationService = notificationService;
        DataBroker = dataBroker;
    }

    public async ValueTask<bool> GetFkList()
    {
        if (_firstLoad)
            this.NotificationService.ListUpdated += OnUpdate;

        _firstLoad = false;
        var cancel = new CancellationToken();
        var result = await this.DataBroker.GetRecordsAsync<TFkRecord>(new ListProviderRequest( 0, 10000, cancel));
        _fkList = result.Items;
        return result.Success;
    }

    public async void OnUpdate(object? sender, EventArgs e)
    {
        await this.GetFkList();
    }

    public void Dispose()
    {
        if (this.NotificationService is not null)
            this.NotificationService.ListUpdated -= OnUpdate;
    }
}

