# List Forms

Forms that present lists of information present some difficult challenges.

They need to support:

1. Paging.
2. Column Sorting.
3. Data Filtered.
4. State : return to the same sorted page after visiting a record view dialog or page.

This solution implements a `ListContext` to manage lists.  A similar framework to the `EditContext` in edit forms.

## State Services

Before we dive into the forms, we need to look at how the application manages state.  I've shied away from **Fluxor**, it's just too complicated for smaller applications.  Instead the application implements a simple `IUIStateManagementService` that maintains a dictionary of Guid/Object pairs.  The form is presented by the Guid and it's state stored in a `record`.  Each application route has a Guid,  specified like this:

```csharp
@page "/weatherforecast/list"
@namespace Blazr.App.UI

<WeatherForecastPagedListForm RouteId=RouteId class="mt-2"/>

@code {
    private static Guid RouteId = Guid.NewGuid();
}
```

`RouteId` is static so there's a single Id for the route while the application runnng.  The route passes this to the form it uses through a `Parameter`.  You need one Guid for each form that needs to implement state management in a route.

`IUIStateManagementService` is a *Scoped* service, so the `RouteId` is unique.

First our interface which implements a getter and a setter for a Guid and a clear method.

```csharp
namespace Blazr.UI;

public interface IUiStateService
{
    public void AddStateData(Guid Id, object value);
    public void ClearStateDataData(Guid Id);
    public bool TryGetStateData<T>(Guid Id, out T? value);
}
```

The implementation maintains state in-application with a Scoped service.

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

## ListForm

`ListForm` is the list equivalent of `EditForm`.  It cascades the form `ListContext` so all the list components have access to the form's `ListContext` instance.  These include the sort controls in the column headers and the paging control.

```csharp
public class ListForm : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] [EditorRequired] public ListContext? ListContext { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<ListContext>>(0);
        builder.AddAttribute(1, "Value", this.ListContext);
        builder.AddAttribute(2, "IsFixed", true);
        builder.AddAttribute(3, "ChildContent", ChildContent);
        builder.CloseComponent();
    }
}
```

## ListContext

You can see the full class in the Appendix.  Here I'm breaking it down in blocks of code that provide certain key functionality.

At this point it's important to understand that `ListContext` doesn't hold the display record set.  That is held by the List View Service.  It manages the UI context state.   

Associated with `ListContext` is `ListState` which is a record used to pass state data to other processes.

```csharp
public record ListState
{
    public string? SortField { get; init; }
    public bool SortDescending { get; init; }
    public int PageSize { get; init; } = 1000;
    public int StartIndex { get; init; } = 0;
    public int ListTotalCount { get; init; } = 0;
    public string SortExpression { get; init; } = string.Empty;
}
```

#### Initialization

`ListContext` is initialized by calling `Load`.  It sets the list provider delegate and the Uid for state management and retrieves the saved state if it exists.

```csharp
internal void Load(Guid stateId, Func<ListState, ValueTask<(int, bool)>>? listProvider)
{
    _stateId = stateId;
    _listProvider = listProvider;
    _hasLoaded = true;

    this.GetState();
}
```

This is Load being set in the List Form:

```csharp
    this.ListContext.Load(this.RouteId, GetPagedItems);
```

#### The List Provider

The list provider is a delegate the class calls whenever it's state changes.  The caller passes the registered delegate a `ListState` instance and provider returns a `tuple` containing the list total count and a success/failure flag. 

The total list count is important for paging.  The ListService Record property will contain the actual records, but these are just the requested page.  The paging control needs to know to total records to work. 

#### State Management

There are three methods:

1. `GetState` tries to retrieve the current state data if it exists.
2. `SaveState` saves the current state.
3. `CheckState` checks if there is a saved state.

```csharp
public bool GetState()
{
    var result =  _uiStateService?.TryGetStateData<ListState>(_stateId, out ListState? state) ?? false
            ? this.Set(state)
            : false;

    _currentRecord = this.ListStateRecord;
    return result;
}

public void SaveState()
{
    _uiStateService?.AddStateData(_stateId, this.ListStateRecord);
    _currentRecord = this.ListStateRecord;
}

public bool CheckState()
    => _uiStateService?.TryGetStateData<ListState>(_stateId, out ListState? state) ?? false;
```

#### Paging

First the request object that defines any paging request:

```csharp
public record PagingRequest
{
    public int PageSize { get; init; } = 1000;
    public int StartIndex { get; init; } = 0;
    public int Page => StartIndex <= 0
        ? 0
        : StartIndex / PageSize;

    public PagingRequest() { }

    public PagingRequest(int page)
        => this.StartIndex = PageSize * 0;
}
```

There are four public properties:

```csharp
    public int PageSize { get; set; } = 1000;
    public int StartIndex { get; set; } = 0;
    public int ListTotalCount { get; set; } = 0;
    public int Page => StartIndex / PageSize;
```

The default settings effectively turn off paging, but restrain queries to 1000 records.

`ListContext` provides the infrastructure for paging.  It doesn't implement paging.  In the application this is done by `PagingControl` which we'll look at later.

Paging controls call `PageAsync` to initiate paging.  The method:

1. Checks we have loaded the object.
2. Sets the internal properties to the values provided in the `PagingRequest`.
3. Calls the list provider if one is defined.
4. Checks for a valid result and sets the `ListTotalCount`. 

```csharp
public async ValueTask<bool> PageAsync(PagingRequest? request = null)
{
    if (!_hasLoaded)
        throw new InvalidOperationException("You can't use the ListContext until you have loaded it.");

    this.Set(request);

    if (_listProvider is not null)
    {
        var result = await _listProvider(this.ListStateRecord);
        if (result.Item2)
            this.ListTotalCount = result.Item1;

        this.SaveState();

        return result.Item2;
    }

    return false;
}

//...
public void Set(PagingRequest? request)
{
    if (request is not null)
    {
        this.StartIndex = request.StartIndex;
        this.PageSize = request.PageSize;
    }
}
```

#### Sorting

First the request object that defines any sorting request:

```csharp
public record SortRequest
{
    public string? SortField { get; init; }
    public bool SortDescending { get; init; }
    public bool IsSorting => !string.IsNullOrWhiteSpace(SortField);
    public string SortExpression => $"{SortField}{sortDirectionText}";
    private string sortDirectionText => SortDescending ? " Desc" : string.Empty;
}
```

There are four properties:

```csharp
    private string sortDirectionText => SortDescending ? " Desc" : string.Empty;
    public string? SortField { get; set; }
    public bool SortDescending { get; set; }
    public string SortExpression => $"{SortField}{sortDirectionText}";
```

`ListContext` provides the infrastructure for sorting.  It doesn't implement sorting.  In the application this is done by sorting controls in the column headers.

Sorting controls call `SortAsync` to initiate sorting.  The method:

1. Checks we have loaded the object.
2. Sets the internal properties to the values provided in the `SortRequest`.
3. Calls the list provider if one is defined.
4. Checks for a valid result and invokes `PagingReset` if necessary. 

```csharp
public async ValueTask SortAsync(SortRequest request)
{
    if (!_hasLoaded)
        throw new InvalidOperationException("You can't use the ListContext untill you have loaded it.");

    this.GetState();

    var reset = this.Set(request);

    if (_listProvider is not null)
    {
        var returnState = await _listProvider(this.ListStateRecord);

        if (returnState.Item2)
            this.ListTotalCount = returnState.Item1;
    }

    this.SaveState();

    if (reset)
        PagingReset?.Invoke(this, new PagingEventArgs(this.PagingState));
}
}

//...
public bool Set(SortRequest? request)
{
    if (request is not null)
    {
        bool isPagingReset = !request.SortField?.Equals(this.SortField) ?? false;

        this.SortField = request.SortField;
        this.SortDescending = request.SortDescending;

        // If we are sorting on a new field then we need to reset the page
        if (isPagingReset)
            this.StartIndex = 0;

        return isPagingReset;
    }
    return false;
}
```

## BlazrPagedListForm

The class is too large to show in full.  I'll concentrate on the key features and logic here.

The class declaration:

```csharp
public abstract class BlazrPagedListForm<TRecord, TEntity>
    : BlazrOwningComponentBase<IListService<TRecord, TEntity>>, IDisposable
    where TRecord : class, new()
    where TEntity : class, IEntity
{
//....
}
```

This is an abstact class with two generic types:

- `TRecord` is the data record.
- `TEntity` is the entity class.

The class inherits from `BlazrOwningComponentBase` which is a custom version of `OwningComponentBase`.  This restricts the `IListService` service scope to the lifetime of the component.  The consequence of this is that we need to set the associated injected services to the the SPA scope.  We do this by calling `SetServices` on `IListServices`.

```csharp
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        //....
        if (_isNew)
            this.Service.SetServices(SPAServiceProvider);
        //...
    }
```

#### DI Service instances

We inject a whole set of service instances we need:

``` csharp
    [Inject] protected INotificationService<TEntity> NotificationService { get; set; } = default!;
    [Inject] protected IEntityService<TEntity> EntityService { get; set; } = default!;
    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IUiStateService UiStateService { get; set; } = default!;
    [Inject] protected ModalService ModalService { get; set; } = default!;
    [Inject] protected ListContext ListContext { get; set; } = default!;
    [Inject] protected IServiceProvider SPAServiceProvider { get; set; } = default!;
```

The component overrides `SetParametersAsync`.

```csharp
public override async Task SetParametersAsync(ParameterView parameters)
{
    parameters.SetParameterProperties(this);

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
        this.ListContext.PageSize = this.PageSize;
        this.ListContext.Load(this.RouteId, GetPagedItems);
        this.ListContext.SortDescending = this.EntityUIService.DefaultSortDescending;
        this.ListContext.SortField = this.EntityUIService.DefaultSortField;

        this.NotificationService.ListUpdated += this.OnListChanged;
    }

    await base.SetParametersAsync(ParameterView.Empty);
    _isNew = false;
}
```


