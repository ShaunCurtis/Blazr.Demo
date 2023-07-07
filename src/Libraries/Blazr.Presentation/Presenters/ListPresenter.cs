/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Presentation;

public class ListPresenter<TRecord, TEntityService>
    : IListPresenter<TRecord, TEntityService>, IListEventConsumer<TRecord>, IDisposable
    where TRecord : class, new()
    where TEntityService : class, IEntityService
{
    private readonly IDataBroker _dataBroker;
    private readonly IUiStateService _uiStateService;
    protected readonly IListController<TRecord> _listController;
    private readonly INotificationService<TEntityService> _notificationService;

    public IListController<TRecord> ListController => _listController;

    public Guid StateId { get; set; } = Guid.NewGuid();

    public readonly Guid InstanceId = Guid.NewGuid();
    public int DefaultPageSize { get; set; } = 1000;
    public IEnumerable<FilterDefinition> DefaultFilters { get; set; } = Enumerable.Empty<FilterDefinition>();

    public ListPresenter(IDataBroker dataBroker, IUiStateService uiStateService, IListController<TRecord> controller, INotificationService<TEntityService> notificationService)
    {
        _dataBroker = dataBroker;
        _uiStateService = uiStateService;
        _listController = controller;
        _notificationService= notificationService;
        
        _listController.RegisterForEvents(this as IListEventConsumer<TRecord>);
        _notificationService.RecordChanged += this.OnListUpdated;
    }

    public void SetFilter(FilterRequest<TRecord> request)
        => this.DefaultFilters = request.Filters;

    public ValueTask GetItemsAsync(ListQueryRequest request, object? sender = null)
        => this.getItemsAsync(request, sender);

    public ValueTask<ItemsProviderResult<TRecord>> GetItemsAsync(ItemsProviderRequest itemsRequest)
        => getItemsAsync(itemsRequest);

    protected virtual async ValueTask getItemsAsync(ListQueryRequest request, object? sender = null)
    {
        ListQueryResult<TRecord> result = await _dataBroker.GetItemsAsync<TRecord>(request);

        // Check if the requested page is beyond the count
        // We may have filtered down to a much small list
        // If so reset the request and requery to get the first page
        if (result.Successful && request.StartIndex >= result.TotalCount)
        {
            request = request with { StartIndex = 0 };
            result = await _dataBroker.GetItemsAsync<TRecord>(request);
        }

        if (result.Successful)
        {
            _listController.Set(request, result);
            _listController.NotifyStateChanged(sender);
            this.SaveState();
        }

        _listController.NotifyStateChanged(sender);
    }

    protected virtual async ValueTask<ItemsProviderResult<TRecord>> getItemsAsync(ItemsProviderRequest itemsRequest)
    {
        var request = new ListQueryRequest() { StartIndex = itemsRequest.StartIndex, PageSize = itemsRequest.Count };

        ListQueryResult<TRecord> result = await _dataBroker.GetItemsAsync<TRecord>(request);

        return result.Successful
            ? new ItemsProviderResult<TRecord>(result.Items, result.TotalCount > int.MaxValue ? int.MaxValue : (int)result.TotalCount)
            : new ItemsProviderResult<TRecord>(Enumerable.Empty<TRecord>(), 0);
    }

    public async ValueTask PagingRequestedAsync(object? sender, PagingEventArgs request)
        => await this.GetItemsAsync(this.GetListProviderRequest(request), sender);

    public async ValueTask SortingRequested(object? sender, SortEventArgs request)
        => await this.GetItemsAsync(this.GetListProviderRequest(request), sender);

    public async ValueTask FilteringRequested(object? sender, FilterEventArgs<TRecord> request)
        => await this.GetItemsAsync(this.GetListProviderRequest(request), sender);

    // Handles events raised by the Notification Service
    public async void OnListUpdated(object? sender, EventArgs e)
        => await this.PagingRequestedAsync(sender, new PagingEventArgs(this.ListController.ListState.GetAsPagingRequest()));

    protected ListQueryRequest GetListProviderRequest(PagingEventArgs eventArgs)
    {
        ListState<TRecord> listState = this._listController.ListState;

        if (eventArgs.Request != null)
        {
            listState.SetPaging(eventArgs.Request);
            return listState.GetListQueryRequest();
        }

        if (this.TryGetState(this.StateId, out ListState<TRecord>? state))
        {
            listState.SetFromListState(state);
            return listState.GetListQueryRequest();
        }

        var defaultSort = new SortRequest();

        var defaultPaging = new PagingRequest { PageSize = eventArgs.InitialPageSize ?? this.DefaultPageSize };
        listState.Set(defaultPaging, defaultSort, DefaultFilters);

        return listState.GetListQueryRequest();
    }

    protected ListQueryRequest GetListProviderRequest(SortEventArgs request)
    {
        this._listController.ListState.SetSorting(request.Request ?? new SortRequest());
        return this._listController.ListState.GetListQueryRequest();
    }

    protected ListQueryRequest GetListProviderRequest(FilterEventArgs<TRecord> request)
    {
        this._listController.ListState.SetFiltering(request.Request ?? new FilterRequest<TRecord>());
        return this._listController.ListState.GetListQueryRequest();
    }

    private bool TryGetState(Guid stateId, [NotNullWhen(true)] out ListState<TRecord>? state)
    {
        var result = _uiStateService.TryGetStateData<ListState<TRecord>>(this.StateId, out ListState<TRecord>? listState);
        state = listState;
        return result;
    }

    private void SaveState()
        => _uiStateService?.AddStateData(this.StateId, this._listController.ListState);

    public void Dispose()
        => _notificationService.RecordChanged -= this.OnListUpdated;
}
