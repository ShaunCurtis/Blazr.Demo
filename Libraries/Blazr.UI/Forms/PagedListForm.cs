/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public class PagedListForm<TRecord, TEntity>
    : OwningComponentBase<IListService<TRecord, TEntity >>, IDisposable
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected IPagingControl? pagingControl;
    private bool _isNew = true;
    protected ListContext listContext = new ListContext();
    protected Type? ViewControl;
    protected Type? EditControl;
    protected bool isLoading => Service.Records is null;
    protected ComponentState loadState => isLoading ? ComponentState.Loading : ComponentState.Loaded;

    public Func<TRecord, bool>? ListFilter { get; set; }

    [Parameter] public Guid RouteId { get; set; } = Guid.Empty;

    [Parameter] public bool IsSubForm { get; set; } = false;

    [Parameter] public int PageSize { get; set; } = 20;

    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

    [Inject] protected INotificationService<TEntity> NotificationService { get; set; } = default!;

    [Inject] protected IEntityService<TEntity> EntityService { get; set; } = default!;

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject] protected ToasterService ToasterService { get; set; } = default!;

    [Inject] protected UiStateService UiStateService { get; set; } = default!;

    [Inject] protected ModalService ModalService { get; set; } = default!;

    [Parameter] public bool UseModalForms { get; set; } = true;

    protected string FormCss => new CSSBuilder()
        .AddClassFromAttributes(UserAttributes)
        .Build();

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        this.Service.SetNotificationService(this.NotificationService);

        await PreLoadRecordAsync(_isNew);

        if (_isNew)
            this.NotificationService.ListUpdated += this.OnListChanged;

        await base.SetParametersAsync(ParameterView.Empty);
        _isNew = false;
    }

    public virtual Task PreLoadRecordAsync(bool isNew)
        => Task.CompletedTask;

    public virtual async ValueTask<PagingState> GetPagedItems(PagingState request)
    {
        var listState = new ListState { PageSize = request.PageSize, StartIndex = request.StartIndex };
        var newListState = await this.GetPagedItems(listState);

        return new PagingState { PageSize = newListState.PageSize, StartIndex = newListState.StartIndex, ListTotalCount = newListState.ListTotalCount };
    }

    public async ValueTask<ListState> GetPagedItems(ListState request)
    {
        var listState = this.GetState(request);

        var query = new FilteredListQuery<TRecord>(new ListProviderRequest<TRecord>(listState, this.ListFilter));
        var result = await this.Service.GetRecordsAsync(query);
        
        listState.ListTotalCount = result.TotalItemCount;
        
        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);
        
        this.SaveState(listState);
        
        return listState;
    }

    protected virtual Task OnAfterGetItems()
        => Task.CompletedTask;

    protected void SaveState(ListState state)
    {
        if (this.RouteId != Guid.Empty)
            this.UiStateService.AddStateData(this.RouteId, state);
    }

    protected ListState GetState(ListState state)
    {
        ListState? returnState = null;
        // TODO - can reduce this if the new version of TryGetStateData works
        if (this.RouteId != Guid.Empty && this.UiStateService.TryGetStateData<ListState>(this.RouteId, out object? listState) && listState is ListState)
        {
            returnState = (listState as ListState)!.Copy;
            if (!_isNew)
            {
                returnState.StartIndex = state.StartIndex;
                returnState.PageSize = state.PageSize;
            }
        }
        _isNew = false;
        returnState ??= state;
        return returnState;
    }
    protected virtual void RecordDashboard(Guid Id)
        => this.NavigationManager!.NavigateTo($"/{this.EntityService.Url}/dashboard/{Id}");


    protected async Task EditRecord(Guid Id)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.EditControl is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", Id);
            options = this.GetEditOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EditControl, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.EntityService.Url}/edit/{Id}");
    }

    protected async Task ViewRecord(Guid Id)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.ViewControl is not null)
        {
            var options = GetViewOptions(null);
            options.ControlParameters.Add("Id", Id);
            await this.ModalService.Modal.ShowAsync(this.ViewControl,  options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.EntityService.Url}/view/{Id}");
    }

    protected async Task AddRecordAsync(ModalOptions? options = null)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.EditControl is not null)
        {
            options = this.GetAddOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EditControl, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.EntityService.Url}/edit/0");
    }

    protected virtual ModalOptions GetAddOptions(ModalOptions? options)
        => options ?? new ModalOptions();

    protected virtual ModalOptions GetEditOptions(ModalOptions? options)
        => options ?? new ModalOptions();

    protected virtual ModalOptions GetViewOptions(ModalOptions? options)
        => options ?? new ModalOptions();

    protected virtual void Exit()
        => this.NavigationManager.NavigateTo("/");

    protected virtual void ExitTo(string url)
        => this.NavigationManager.NavigateTo(url);

    private void OnListChanged(object? sender, EventArgs e)
    {
        this.pagingControl?.NotifyListChangedAsync();
        this.InvokeAsync(this.StateHasChanged);
    }
    public virtual void Dispose()
        => this.NotificationService.ListUpdated += this.OnListChanged;
}
