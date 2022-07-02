/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class StandardForeignKeyService<TFkRecord, TService>
    : IForeignKeyService<TFkRecord, TService>, IDisposable
        where TFkRecord : class, IFkListItem, new()
        where TService : class, IEntityService
{
    protected INotificationService<TService> NotificationService;
    protected ICQSDataBroker DataBroker;
    private bool _firstLoad = true;

    public IEnumerable<IFkListItem> Items { get; protected set; } = Enumerable.Empty<TFkRecord>();

    public StandardForeignKeyService(ICQSDataBroker dataBroker, INotificationService<TService> notificationService)
    {
        NotificationService = notificationService;
        DataBroker = dataBroker;
    }

    public async ValueTask<bool> GetFkList()
    {
        if (_firstLoad)
            this.NotificationService.ListUpdated += OnUpdate;

        _firstLoad = false;
        var result = await this.DataBroker.ExecuteAsync<TFkRecord>(new FKListQuery<TFkRecord>());
        this.Items = result.Items;
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

