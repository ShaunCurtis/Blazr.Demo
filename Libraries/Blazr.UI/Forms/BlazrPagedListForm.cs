/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public abstract class BlazrPagedListForm<TRecord, TEntity>
    : BlazrOwningComponentBase<IListService<TRecord, TEntity>>, IDisposable
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected IPagingControl? pagingControl;
    protected string FormTitle = "Record Editor";
    protected string NewRecordText = "Add Record";
    private bool _isNew = true;
    protected virtual Type? ViewControl => this.EntityUIService.ViewForm;
    protected virtual Type? EditControl => this.EntityUIService.EditForm;
    protected bool isLoading => Service.Records is null;
    protected ComponentState loadState => isLoading ? ComponentState.Loading : ComponentState.Loaded;

    public Func<TRecord, bool>? ListFilter { get; set; }

    [Parameter] public Guid RouteId { get; set; } = Guid.Empty;

    [Parameter] public bool IsSubForm { get; set; } = false;

    [Parameter] public int PageSize { get; set; } = 20;

    [Parameter] public bool UseModalForms { get; set; } = true;

    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

    [Inject] protected INotificationService<TEntity> NotificationService { get; set; } = default!;

    [Inject] protected IEntityService<TEntity> EntityService { get; set; } = default!;

    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject] protected ToasterService ToasterService { get; set; } = default!;

    [Inject] protected IUiStateService UiStateService { get; set; } = default!;

    [Inject] protected ModalService ModalService { get; set; } = default!;

    [Inject] protected ListContext ListContext { get; set; } = default!;

    [Inject] protected IServiceProvider SPAServiceProvider { get; set; } = default!;

    protected string FormCss => new CSSBuilder()
        .AddClassFromAttributes(UserAttributes)
        .Build();

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        this.Service.SetServices(SPAServiceProvider);

        if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
        {
            this.FormTitle = $"List of {this.EntityUIService.PluralTitle}";
            NewRecordText = $"Add {this.EntityUIService.SingleTitle}";
        }

        await PreLoadRecordAsync(_isNew);

        if (_isNew)
        {
            this.ListContext.PageSize = this.PageSize;
            this.ListContext.Load(this.RouteId, GetPagedItems);
            this.ListContext.SortDescending = this.EntityUIService.DefaultSortDescending;
            this.ListContext.SortField = this.EntityUIService.DefaultSortField;

            this.NotificationService.ListUpdated += this.OnListChanged;
        }

        await base.SetParametersAsync(ParameterView.Empty);
        _isNew = false;
    }

    public virtual Task PreLoadRecordAsync(bool isNew)
        => Task.CompletedTask;

    //public async ValueTask GetPagedItems()
    //{
    //    var query = new FilteredListQuery<TRecord>(new ListProviderRequest<TRecord>(this.ListContext.ListStateRecord, this.ListFilter));
    //    var result = await this.Service.GetRecordsAsync(query);

    //    this.ListContext.ListTotalCount = result.TotalItemCount;

    //    await this.OnAfterGetItems();
    //    await this.InvokeAsync(StateHasChanged);

    //    this.ListContext.SaveState();
    //}

    protected abstract IFilteredListQuery<TRecord> GetFilteredQuery(ListProviderRequest<TRecord> request);
 
    public async ValueTask GetPagedItems()
    {
        var request = new ListProviderRequest<TRecord>(this.ListContext.ListStateRecord);
        var query = this.GetFilteredQuery(request);
        var result = await this.Service.GetRecordsAsync(query);

        this.ListContext.ListTotalCount = result.TotalItemCount;

        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);

        this.ListContext.SaveState();
    }


    public async ValueTask<(int, bool)> GetPagedItems(ListState state)
    {
        var request = new ListProviderRequest<TRecord>(state);
        var query = this.GetFilteredQuery(request);

        var result = await this.Service.GetRecordsAsync(query);

        this.ListContext.ListTotalCount = result.TotalItemCount;

        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);

        this.ListContext.SaveState();

        return (result.TotalItemCount, result.Success);
    }

    //public async ValueTask<(int, bool)> GetPagedItems(ListState request)
    //{
    //    var query = new FilteredListQuery<TRecord>(new ListProviderRequest<TRecord>(request, this.ListFilter));
    //    var result = await this.Service.GetRecordsAsync(query);

    //    this.ListContext.ListTotalCount = result.TotalItemCount;

    //    await this.OnAfterGetItems();
    //    await this.InvokeAsync(StateHasChanged);

    //    this.ListContext.SaveState();

    //    return (result.TotalItemCount, result.Success);
    //}

    protected virtual Task OnAfterGetItems()
        => Task.CompletedTask;

    protected virtual void RecordDashboard(Guid Id)
        => this.NavigationManager!.NavigateTo($"/{this.EntityUIService.Url}/dashboard/{Id}");

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
            this.NavigationManager!.NavigateTo($"/{this.EntityUIService.Url}/edit/{Id}");
    }

    protected async Task ViewRecord(Guid Id)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.ViewControl is not null)
        {
            var options = GetViewOptions(null);
            options.ControlParameters.Add("Id", Id);
            await this.ModalService.Modal.ShowAsync(this.ViewControl, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.EntityUIService.Url}/view/{Id}");
    }

    protected async Task AddRecordAsync(ModalOptions? options = null)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.EditControl is not null)
        {
            options = this.GetAddOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EditControl, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.EntityUIService.Url}/edit/0");
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
