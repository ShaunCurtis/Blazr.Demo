/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public class ForeignKeyPresenter<TFkItem, TEntityService>
    : IForeignKeyPresenter<TFkItem, TEntityService>, IDisposable
        where TFkItem : class, IFkItem, new()
        where TEntityService : class, IEntityService
{
    protected INotificationService<TEntityService> NotificationService;
    protected IDataBroker DataBroker;
    private bool _firstLoad = true;

    public Task LoadTask { get; private set; } = Task.CompletedTask;

    public IEnumerable<IFkItem> Items { get; protected set; } = Enumerable.Empty<TFkItem>();

    public ForeignKeyPresenter(IDataBroker dataBroker, INotificationService<TEntityService> notificationService)
    {
        NotificationService = notificationService;
        DataBroker = dataBroker;
        LoadTask = Load();
    }

    public async Task<bool> Load()
    {
        if (_firstLoad)
            this.NotificationService.ListChanged += OnUpdate;

        _firstLoad = false;
        var result = await this.DataBroker.GetItemsAsync<TFkItem>(new ListQueryRequest());
        this.Items = result.Items;

        return result.Successful;
    }

    public async void OnUpdate(object? sender, EventArgs e)
        => await this.Load();

    public void Dispose()
    {
        if (this.NotificationService is not null)
            this.NotificationService.ListChanged -= OnUpdate;
    }
}

