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
    protected IListService<TRecord> ListViewService => this._listViewService!;
    protected INotificationService<TService> NotificationService => _notificationService!;
    protected NavigationManager NavigationManager => _navigationManager!;
    protected ToasterService ToasterService => _toasterService!;
    protected UiStateService UiStateService => _uiStateService!;
    protected ModalService modalService => _modalService!;
    protected bool isLoading => ListViewService.Records is null;
    protected ComponentState loadState => isLoading ? ComponentState.Loading : ComponentState.Loaded;

    [Parameter] public Guid RouteId { get; set; } = Guid.Empty;

    [Parameter] public bool IsSubForm { get; set; } = false;

    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

    [Inject] private IListService<TRecord>? _listViewService { get; set; }

    [Inject] private INotificationService<TService>? _notificationService { get; set; }

    [Inject] private NavigationManager? _navigationManager { get; set; }

    [Inject] private ToasterService? _toasterService { get; set; }

    [Inject] private UiStateService? _uiStateService { get; set; }

    [Inject] private ModalService? _modalService { get; set; }

    public bool UseModalForms { get; set; } = false;

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

    public virtual async ValueTask<PagingOptions> GetPagedItems(PagingOptions request)
    {
        var listOptions = new ListOptions { PageSize = request.PageSize, StartIndex = request.StartIndex };
        listOptions = this.GetState(listOptions);
        var result = await this.ListViewService.GetRecordsAsync(new ListProviderRequest(listOptions));
        listOptions.ListTotalCount = result.TotalItemCount;
        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);
        this.SaveState(listOptions);
        return new PagingOptions { PageSize = listOptions.PageSize, StartIndex = listOptions.StartIndex, ListTotalCount = listOptions.ListTotalCount };
    }

    public async ValueTask<ListOptions> GetPagedItems(ListOptions request)
    {
        var listOptions = this.GetState(request);
        var result = await this.ListViewService.GetRecordsAsync(new ListProviderRequest(listOptions));
        listOptions.ListTotalCount = result.TotalItemCount;
        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);
        this.SaveState(listOptions);
        return listOptions;
    }

    protected virtual Task OnAfterGetItems()
        => Task.CompletedTask;

    protected void SaveState(ListOptions options)
    {
        if (this.RouteId != Guid.Empty)
            this.UiStateService.AddStateData(this.RouteId, options);
    }

    protected ListOptions GetState(ListOptions options)
    {
        ListOptions? returnOptions = null;
        // TODO - can reduce this if the new version of TryGetStateData works
        if (this.RouteId != Guid.Empty && this.UiStateService.TryGetStateData<ListOptions>(this.RouteId, out object? stateOptions) && stateOptions is ListOptions)
        {
            returnOptions = (stateOptions as ListOptions)!.Copy;
            if (!_firstLoad)
            {
                returnOptions.StartIndex = options.StartIndex;
                returnOptions.PageSize = options.PageSize;
            }
        }
        _firstLoad = false;
        returnOptions ??= options;
        return returnOptions;
    }


    protected virtual async Task EditRecord(Guid Id)
    {
        if (this.modalService.IsModalFree && this.UseModalForms && this.EditControl is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", Id);
            options = this.GetEditOptions(options);
            await this.modalService.Modal.ShowAsync(this.EditControl, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.RecordUrl}/edit/{Id}");
    }

    protected virtual async Task ViewRecord(Guid Id)
    {
        if (this.modalService.IsModalFree && this.UseModalForms && this.ViewControl is not null)
        {
            var options = GetViewOptions(null);
            options.ControlParameters.Add("Id", Id);
            await this.modalService.Modal.ShowAsync(this.ViewControl,  options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.RecordUrl}/view/{Id}");
    }

    protected virtual async Task AddRecordAsync(ModalOptions? options = null)
    {
        if (this.modalService.IsModalFree && this.UseModalForms && this.EditControl is not null)
        {
            options = this.GetAddOptions(options);
            await this.modalService.Modal.ShowAsync(this.EditControl, options);
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
    {
        this.NavigationManager.NavigateTo("/");
    }

    protected virtual void ExitTo(string url)
    {
        this.NavigationManager.NavigateTo(url);
    }

    private void OnListChanged(object? sender, EventArgs e)
    {
        this.pagingControl?.NotifyListChangedAsync();
        this.InvokeAsync(this.StateHasChanged);
    }

    public virtual void Dispose()
    {
        this.NotificationService.ListUpdated += this.OnListChanged;
    }
}
