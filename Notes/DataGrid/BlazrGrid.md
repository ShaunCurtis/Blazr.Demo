# Building a Blazor Grid Component

Building your own Blazor grid component is a significant challenge for many programmers: you need a very good working knowledge of the component architecture.  

This article describes how to put together the building blocks you need and code the component rendering process to build the grid markup.

The full code base is in the repository.  Only the more important code fragments are shown in the article.

## The Finished Page

It's worth looking at the finished page before going into the detail.

1. The component injects a generic `IListPresenter`.  The presenter is a Presentation layer object of the Core/Application Domain.  It interfaces with the data pipeline, and manages the list data for the Form/Page component.

2. The component inherits from `UIControlBase`,  a custom component implementing a single lifecycle method.

3. The presenter's `ListComponentController` instance is cascaded into the form and captured by various components within the form such as the paging control and sorting controls in the columns.

4. A set of `BlazrGridColumn` and `BlazrSortedGridColumn` components defined in the `BlazrGrid` component's child content that define the display columns.

```html
@page "/"
@namespace Blazr.App.UI
@using Blazr.UI.BlazrGrid
@inherits UIControlBase
@inject IListPresenter<WeatherForecast> Presenter

<PageTitle>Index</PageTitle>

<CascadingValue Value=this.Presenter.ListController IsFixed=true>
    <div class="container-fluid mb-2">
        <div class="row">
            <div class="col-12 col-lg-8">
                <BlazrPagingControl TRecord="WeatherForecast" DefaultPageSize="20" />
            </div>
            <div class="col-12 col-lg-4">
                <SummaryFilter />
            </div>
        </div>
    </div>
    <BlazrGrid TGridItem="WeatherForecast">
        <BlazrSortedGridColumn TGridItem="WeatherForecast" SortField="Date" IsNoWrap=true Title="Date">@context.Date.AsGlobalDate()</BlazrSortedGridColumn>
        <BlazrSortedGridColumn TGridItem="WeatherForecast" SortField="TemperatureC" IsNoWrap=true class="text-end pe-2" Title="Temperature">
            @(context.TemperatureC)@((MarkupString)"&deg;C") / @(context.AsTemperatureF())@((MarkupString)"&deg;F")
            </BlazrSortedGridColumn>
        <BlazrSortedGridColumn TGridItem="WeatherForecast" IsMaxColumn=true SortField="Summary" Title="Summary">@context.Summary</BlazrSortedGridColumn>
        <BlazrGridColumn TGridItem="WeatherForecast" Title="Actions" Class="text-end">
            <button class="btn btn-sm btn-primary" @onclick="() => Click(context)">Edit</button>
        </BlazrGridColumn>
    </BlazrGrid>

</CascadingValue>
```
```csharp
@code {
    private void Click(WeatherForecast record)
    {
        // Do something
    }
}
```

## The Form/Page Classes

The `IListPresenter` interface provides the functionality to interact with the data.  The `ListPresenter` implementation is in the repo.

There are two `GetItemsAsync` methods to load data.  One is designed to interface with the `Virtualize` component and the other to load the data set into the presenter. 

```csharp
public interface IListPresenter<TRecord> where TRecord : class, new()
{
    public ListController<TRecord> ListController { get; }
    public int DefaultPageSize { get; set; }
    public ValueTask GetItemsAsync(ListQueryRequest<TRecord> request, object? sender = null);
    public ValueTask<ItemsProviderResult<TRecord>> GetItemsAsync(ItemsProviderRequest itemsRequest);
}
```

The `ListController` is the "command and control object" for list operations.  It provides:

1. A set of events to drive component activity such as renders.
2. A set of notifiers to invoke the events.
3. The data set list which is exposed directly through the controller's `IEnurerable` implementation.
4. A state object that holds the current configuration of the data set such as paging, sorting and filtering.  This object can be persisted.

The class outline looks like this.  Most of the code is self-evident.

```csharp
public sealed class ListController<TRecord> : IEnumerable<TRecord>
    where TRecord : class, new()
{
    private ILogger<ListController<TRecord>> _logger;
    private List<TRecord>? _items = new List<TRecord>();

    private IListEventConsumer<TRecord>? _consumer;
    private IListPagerProvider? _pagerProvider;
    private IListFilterProvider? _filterProvider;

    public readonly Guid Id = Guid.NewGuid();

    public event EventHandler<EventArgs>? StateChanged;

    public readonly ListState<TRecord> ListState = new ListState<TRecord>();
    public bool IsPaging;
    public bool HasList;
    public bool HasPager;

    public ListController(ILogger<ListController<TRecord>> logger)
        => _logger = logger;

    public void RegisterPager(IListPagerProvider? pager);
    public void RegisterForEvents(IListEventConsumer<TRecord>? consumer);
    public void RegisterFilter(IListFilterProvider? filter);
    

    public void Set(ListQueryRequest<TRecord> request, ListQueryResult<TRecord> result);
    public void Reset();

    public async ValueTask NotifyPagingRequestedAsync(object? sender, PagingEventArgs request);
    public async ValueTask NotifySortingRequestedAsync(object? sender, SortEventArgs request);
    public async ValueTask NotifyFilteringRequestedAsync(object? sender, FilterEventArgs<TRecord> request);
    public void NotifyStateChanged(object? sender);

    public IEnumerator<TRecord> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator();
}
```

`IListContextEventConsumer` provides a mechanism for the instance creator - normally the presenter - to receive notifications without wiring up event handlers and therefore needing to implement `IDisposable`.

```csharp
public interface IListContextEventConsumer
{
    public ValueTask PagingRequestedAsync(object? sender, PagingEventArgs request);
    public ValueTask SortingRequested(object? sender, SortEventArgs request);
}
```
The `IListFilterProvider` and `IListPagerProvider` interfaces provide empty interfaces to identify classes

```csharp
public interface IListFilterProvider {}

public interface IListPagerProvider {}
```

A `ListState` instance defines the data needed to reconstruct the record set.

```csharp
public class ListState<TRecord>
    where TRecord : class, new()
{
    public string? SortField { get; private set; }
    public bool SortDescending { get; private set; }

    public int PageSize { get; private set; } = 1000;
    public int StartIndex { get; private set; } = 0;
    public int ListTotalCount { get; private set; } = 0;
    public int Page => StartIndex / PageSize;

    public Expression<Func<TRecord, bool>>? FilterExpression { get; private set; }
    public Expression<Func<TRecord, object>>? SortExpression { get; private set; }

    // Lots of setter and getter methods
}
```

## Grid Design

Grid column components are defined like this:

```csharp
<BlazrGridColumn TGridItem="WeatherForecast" Title="Date">@context.Date.AsGlobalDate()</BlazrGridColumn>
```

Grid Columns are rendered in the sense that they are added to and maintained in the RenderTree, but they output no markup and therefore have no visible content. Their purpose is to define a data set that describes how they are actually rendered.

The render sequence for `BlazrGrid` is defined in `SetParametersAsync`.

`hasPendingQueuedRender` tracks if a render request is already queued.  If one is then we don't need to queue another one. `hasPendingQueuedRender` is set to `false` when the `_gridRenderFragment` is actually invoked bt the Renderer.

```csharp
if (this.hasPendingQueuedRender)
    return;

this.hasPendingQueuedRender = true;
```
The process then checks if this is the first render.  It only needs to run the column registration render once.  It uses a `TaskCompletionSource` to provide a managed `Task` to ensure the `captureColumnsRenderFragment` render fragment has been completed before the `_gridRenderFragment` is queued.  The `TaskCompletionSource` instance is set to complete at the end of `captureColumnsRenderFragment`.  Once the render is queued it yields to the Task Scheduler which allows processor time for the Renderer to service it's queue and invoke `captureColumnsRenderFragment`.  

```csharp
        if (!_initialized)
        {
            _firstRenderTaskManager = new TaskCompletionSource();
            this.renderHandle.Render(captureColumnsRenderFragment);
            await Task.Yield();
        }
```
It then awaits the Task associated with `TaskCompletionSource` instance to ensure all the columns have registered.  On subsiquent executions, `firstRenderTask` will be complete.

```csharp
await Task.Yield();
await this.firstRenderTask;
```

Finally it queues `_gridRenderFragment` to render the grid columns.

```csharp
this.renderHandle.Render(_gridRenderFragment);
```

`captureColumnsRenderFragment` looks like this.  When it's run by the Renderer, it clears the grid column list, cascades the registration method to all the columns and on completion sets the `TaskCompletionSource` to complete.

```csharp
private RenderFragment captureColumnsRenderFragment => (__builder) =>
{
    _gridColumns.Clear();

    <CascadingValue Value="this.RegisterColumn" IsFixed>
        @ChildContent
    </CascadingValue>

    _firstRenderTaskManager?.TrySetResult();
};
```
Once the rendering, and thus the registering of the columns, is done the component can render the table.  It calls `RenderGrid`.

```csharp
this.RenderGrid();
```

Which looks like this:

```csharp
private void RenderGrid()
{
    if (this.hasPendingQueuedRender)
        return;

    this.hasPendingQueuedRender = true;
    this.renderHandle.Render(_gridRenderFragment);
}
```


`gridRenderFragment` renders the main grid content.  It builds out a standard table.

```csharp
protected virtual RenderFragment gridRenderFragment => (__builder) =>
{
    int rowIndex = 0;

    <table class="@BlazrGridCss.TableCss">
        <thead class="@BlazrGridCss.TableHeaderCss">
            @{
                this.renderHeaderRow(__builder);
            }
        </thead>
        <tbody>
            @foreach (var item in _items)
            {
                this.renderRow(__builder, rowIndex, item);
                rowIndex++;
            }
        </tbody>
    </table>
};
```
`renderHeaderRow` builds out the header columns from the `_gridColumns` collection. It calls the `GetItemHeaderContent` method on the object passing in the `RenderTreeBuilder`.

```csharp
protected virtual void renderHeaderRow(RenderTreeBuilder __builder)
{
    <tr>
        @foreach (var col in _gridColumns)
        {
            col.GetItemHeaderContent(__builder);
        }
    </tr>
}
```
`renderrRow` builds out the header columns from the `_gridColumns` collection. It calls the `GetItemRowContent` method on the object passing in the `RenderTreeBuilder`, the row index number and the specific `TGridItem` orw instance from the datya set.

```csharp
protected virtual void renderRow(RenderTreeBuilder __builder, int rowIndex, TGridItem item)
{
    <tr aria-rowindex="@rowIndex" class="@BlazrGridCss.TableRowCss">
        @foreach (var col in _gridColumns)
        {
            col.GetItemRowContent(__builder, item);
        }
    </tr>
}
```

## BlazrGridColumnBase

The important Grid colum code is implemented in `SetParametersAsync`.


```csharp
    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        this.RegisterComponent();
        return Task.CompletedTask;
    }
```

The component captures the cascaded `Action` and invokes it passing in a `IBlazrGridItem<TGridItem>` that defines the column data.

```csharp
    [CascadingParameter] private Action<IBlazrGridItem<TGridItem>>? Register { get; set; }

    private bool _hasNeverRendered = true;

    private void RegisterComponent()
    {
        if (_hasNeverRendered && Register is not null)
            _renderHandle.Render((builder) =>
            {
                Register.Invoke(this.GridItem);
            });
        _hasNeverRendered = false;
    }
```
