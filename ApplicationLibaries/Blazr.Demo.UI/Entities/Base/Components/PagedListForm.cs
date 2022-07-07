/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.UI;

public class PagedListForm<TRecord, TService>
    : ComponentBase
    where TRecord : class, new()
    where TService : class, IEntityService
{
    protected PagingControl? pagingControl;
    protected bool _firstLoad;
    protected ListContext listContext = new ListContext();
    protected Type? ViewControl;
    protected Type? EditControl;
    protected string RecordUrl;
    protected string RecordTitle;
    protected bool isLoading => ListViewService.Records is null;
    protected ComponentState loadState => isLoading ? ComponentState.Loading : ComponentState.Loaded;

    [Parameter] public Guid RouteId { get; set; } = Guid.Empty;

    [Parameter] public bool IsSubForm { get; set; } = false;

    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

    [Inject] protected IListService<TRecord> ListViewService { get; set; } = default!;

    [Inject] protected INotificationService<TService> NotificationService { get; set; } = default!;

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject] protected ToasterService ToasterService { get; set; } = default!;

    [Inject] protected UiStateService UiStateService { get; set; } = default!;

    [Inject] protected ModalService ModalService { get; set; } = default!;

    [Parameter] public bool UseModalForms { get; set; } = false;

    protected string FormCss => new CSSBuilder()
        .AddClassFromAttributes(UserAttributes)
        .Build();

    public PagedListForm()
    {
        var name = new TRecord().GetType().Name
            .Replace("Dbo", "")
            .Replace("Dvo", "");
        
        this.RecordTitle = name;
        this.RecordUrl = name;
    }

    protected override void OnInitialized()
    {
        _firstLoad = true;
        this.NotificationService.ListUpdated += this.OnListChanged;
    }

    public async ValueTask<PagingState> GetPagedItems(PagingState request)
    {
        var listState = new ListState { PageSize = request.PageSize, StartIndex = request.StartIndex };
        listState = this.GetState(listState);

        var result = await this.ListViewService.GetRecordsAsync(new ListProviderRequest<TRecord>(listState));
        
        listState.ListTotalCount = result.TotalItemCount;
        
        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);
        
        this.SaveState(listState);
        
        return new PagingState { PageSize = listState.PageSize, StartIndex = listState.StartIndex, ListTotalCount = listState.ListTotalCount };
    }

    public async ValueTask<ListState> GetPagedItems(ListState request)
    {
        var listState = this.GetState(request);
        
        var result = await this.ListViewService.GetRecordsAsync(new ListProviderRequest<TRecord>(listState));
        
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
            if (!_firstLoad)
            {
                returnState.StartIndex = state.StartIndex;
                returnState.PageSize = state.PageSize;
            }
        }
        _firstLoad = false;
        returnState ??= state;
        return returnState;
    }

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
            this.NavigationManager!.NavigateTo($"/{this.RecordUrl}/edit/{Id}");
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
            this.NavigationManager!.NavigateTo($"/{this.RecordUrl}/view/{Id}");
    }

    protected async Task AddRecordAsync(ModalOptions? options = null)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.EditControl is not null)
        {
            options = this.GetAddOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EditControl, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.RecordUrl}/edit/0");
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
