# List Forms

Forms that present lists of information present different challenges to edit and read forms.

They need to be:

1. Paged.
2. Sortable.
3. Filtered.
4. Maintain state.  You need to return to the same sorted page after visiting a record view dialog or page.

The demo application implements a `ListContext` - a similar framework to the `EditContext` in edit forms.

State Service

Before we dive into the forms, we need to look at how the application manages state.  I've shied away from using **Fluxor** because I consider it just too complicated for smaller applications.  Instead the application implements a simple `UIStateManagementService` that maintains a dictionary of Guid/Objrct pairs.  The form is presented by the Guid and it's state stored in a `record`.  Each application route has a Guid,  specified like this:

```csharp
@page "/weatherforecast/list"
@namespace Blazr.App.UI

<WeatherForecastPagedListForm RouteId=RouteId class="mt-2"/>

@code {
    private static Guid RouteId = Guid.NewGuid();
}
```

`RouteId` is static so there's a single Id for the route while the application runnng.  The route passes this to the form it uses through a `Parameter`.  You need one Guid for each form that needs toimplement state management in a route.

`UIStateManagementService` is a *Scoped* service, so the `RouteId` is unique.

First our interface which basixcally implements a getter and a setter for a Guid and a clear method.

```csharp
namespace Blazr.UI;

public interface IUiStateService
{
    public void AddStateData(Guid Id, object value);
    public void ClearStateDataData(Guid Id);
    public bool TryGetStateData<T>(Guid Id, out T? value);
}
```

The class implemented maintains state in-application with a Scoped service.  You can implement more permenant storage through a different `IUiStateService` implementation.

```csharp
namespace Blazr.UI;

public class UiStateService 
    :IUiStateService
{
    private Dictionary<Guid, object> _stateItems = new Dictionary<Guid, object>();

    public void AddStateData(Guid Id, object value) 
    {
        if (Id == Guid.Empty)
            return;

        if (_stateItems.ContainsKey(Id))
            _stateItems[Id] = value;
        else
            _stateItems.Add(Id, value);
    }

    public void ClearStateDataData(Guid Id)
    {
        if (Id == Guid.Empty)
            return;

        if (_stateItems.ContainsKey(Id))
            _stateItems.Remove(Id);
    }

    public bool TryGetStateData<T>(Guid Id, out T? value)
    {
        value = default;

        if (Id == Guid.Empty)
            return false;

        var isdata = _stateItems.ContainsKey(Id);

        var val = isdata
            ? _stateItems[Id]
            : default;

        if (val is T)
        {
            value = (T)val;
            return true;
        }

        return false;
    }
}
```

## ListContext

```csharp
public class PagedListForm<TRecord, TEntity>
    : OwningComponentBase<IListService<TRecord, TEntity>>, IDisposable
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected IPagingControl? pagingControl;
    private bool _isNew = true;
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

    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject] protected ToasterService ToasterService { get; set; } = default!;

    [Inject] protected IUiStateService UiStateService { get; set; } = default!;

    [Inject] protected ModalService ModalService { get; set; } = default!;

    [Inject] protected ListContext ListContext { get; set; } = default!;

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
        {
            ListContext.PageSize = this.PageSize;
            ListContext.Load(this.RouteId, GetPagedItems);
            this.NotificationService.ListUpdated += this.OnListChanged;
        }

        await base.SetParametersAsync(ParameterView.Empty);
        _isNew = false;
    }

    public virtual Task PreLoadRecordAsync(bool isNew)
        => Task.CompletedTask;

    //public virtual async ValueTask<PagingState> GetPagedItems(PagingState request)
    //{
    //    ListContext.Set(request);

    //    await this.GetPagedItems();

    //    return this.ListContext.PagingState;
    //}

    public async ValueTask GetPagedItems()
    {
        var query = new FilteredListQuery<TRecord>(new ListProviderRequest<TRecord>(this.ListContext.Record, this.ListFilter));
        var result = await this.Service.GetRecordsAsync(query);

        this.ListContext.ListTotalCount = result.TotalItemCount;

        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);

        this.ListContext.SaveState();
    }

    public async ValueTask<(int, bool)> GetPagedItems(ListState request)
    {
        var query = new FilteredListQuery<TRecord>(new ListProviderRequest<TRecord>(request, this.ListFilter));
        var result = await this.Service.GetRecordsAsync(query);

        this.ListContext.ListTotalCount = result.TotalItemCount;

        await this.OnAfterGetItems();
        await this.InvokeAsync(StateHasChanged);

        this.ListContext.SaveState();

        return (result.TotalItemCount, result.Success);
    }

    protected virtual Task OnAfterGetItems()
        => Task.CompletedTask;

    //protected void SaveState(ListState state)
    //    => this.UiStateService.AddStateData(this.RouteId, state.Record);

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
```