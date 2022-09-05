/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrPagedListForm<TRecord, TEntity>
    : BlazrOwningComponentBase<IListService<TRecord, TEntity>>, IDisposable, IHandleEvent, IHandleAfterRender
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected string FormTitle = "Record Editor";
    protected string NewRecordText = "Add Record";
    private bool _isNew = true;

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

    protected string FormCss 
        => new CSSBuilder()
            .AddClassFromAttributes(UserAttributes)
            .Build();

    protected bool isLoading 
        => Service.Records is null;

    protected ComponentState loadState 
        => isLoading ? ComponentState.Loading : ComponentState.Loaded;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        // Sets the parameters as per the base method
        parameters.SetParameterProperties(this);

        // If this is the first set we pass the SPA Service container set the IListServices to the SPA instances
        if (_isNew)
            this.Service.SetServices(SPAServiceProvider);

        if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
        {
            this.FormTitle = $"List of {this.EntityUIService.PluralTitle}";
            NewRecordText = $"Add {this.EntityUIService.SingleTitle}";
        }

        await PreLoadRecordAsync(_isNew);

        if (_isNew)
        {
            this.ListContext.PagingRequested += this.OnPagingRequested;
            this.ListContext.SortingRequested += this.OnSortingRequested;
            this.NotificationService.ListUpdated += this.OnListChanged;
        }

        await base.SetParametersAsync(ParameterView.Empty);
        _isNew = false;
    }

    public virtual Task PreLoadRecordAsync(bool isNew)
        => Task.CompletedTask;

    protected async void OnPagingRequested(object? sender, PagingRequest? request)
        => await this.GetRecordsAsync(this.GetListProviderRequest(request));

    protected async void OnSortingRequested(object? sender, SortRequest request)
        => await this.GetRecordsAsync(this.GetListProviderRequest(request));

    protected async ValueTask<bool> GetRecordsAsync(ListProviderRequest<TRecord> listProviderRequest)
    {
        var query = this.GetListQuery(listProviderRequest);

        if (await this.Service.GetRecordsAsync(query))
        {
            this.ListContext.NotifyStateChanged(this, this.Service.Records.ListState);
            this.SaveState();
            return true;
        }

        //TODO -  add code if fails to show message
        return false;
    }

    protected ListProviderRequest<TRecord> GetListProviderRequest(PagingRequest? request)
    {
        if (request is null && this.TryGetState(this.RouteId, out ListState? state))
            return new ListProviderRequest<TRecord>(state);

        request ??= new PagingRequest { StartIndex = 0, PageSize = this.PageSize };
        return new ListProviderRequest<TRecord>(this.Service.Records.ListState with { PageSize = request.PageSize, StartIndex = request.StartIndex });
    }

    protected ListProviderRequest<TRecord> GetListProviderRequest(SortRequest request)
        => new ListProviderRequest<TRecord>(this.Service.Records.ListState with { SortDescending = request.SortDescending, SortField =request.SortField });

    protected virtual ListQuery<TRecord> GetListQuery(ListProviderRequest<TRecord> request)
        => new ListQuery<TRecord>(request);

    protected virtual void RecordDashboard(Guid Id)
        => this.NavigationManager!.NavigateTo($"/{this.EntityUIService.Url}/dashboard/{Id}");

    protected async Task LoadEditFormAsync(Guid Id)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.EntityUIService.EditForm is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", Id);
            options = this.GetEditOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EntityUIService.EditForm, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.EntityUIService.Url}/edit/{Id}");
    }

    protected async Task LoadViewFormAsync(Guid Id)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.EntityUIService.ViewForm is not null)
        {
            var options = GetViewOptions(null);
            options.ControlParameters.Add("Id", Id);
            await this.ModalService.Modal.ShowAsync(this.EntityUIService.ViewForm, options);
        }
        else
            this.NavigationManager!.NavigateTo($"/{this.EntityUIService.Url}/view/{Id}");
    }

    protected async Task LoadAddFormAsync(ModalOptions? options = null)
    {
        if (this.ModalService.IsModalFree && this.UseModalForms && this.EntityUIService.EditForm is not null)
        {
            options = this.GetAddOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EntityUIService.EditForm, options);
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
        this.ListContext.NotifyListChanged(this);
        this.InvokeAsync(this.StateHasChanged);
    }

    public bool TryGetState(Guid stateId, [NotNullWhen(true)] out ListState? state)
    {
        var result = UiStateService.TryGetStateData<ListState>(this.RouteId, out ListState? listState);
        state = listState;
        return result;
    }

    public void SaveState()
        => UiStateService?.AddStateData(this.RouteId, this.Service.Records.ListState);


    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        await callback.InvokeAsync(arg);
        Render();
    }

    Task IHandleAfterRender.OnAfterRenderAsync()
        => Task.CompletedTask;

    protected override void Dispose(bool disposing)
    {
        this.ListContext.PagingRequested -= this.OnPagingRequested;
        this.ListContext.SortingRequested -= this.OnSortingRequested;
        this.NotificationService.ListUpdated -= this.OnListChanged;
        base.Dispose(disposing);
    }
}
