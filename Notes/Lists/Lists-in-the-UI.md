# List Forms

> Currently **Work in Progress**

List Forms use a templete component to boilerplate most of the code.

`UIPagedListFormBase` consists of a Razor file containing the Ui render fragments and a code behind file containing the code.

The form inherits from the `UIWrapperBase` component.  It defines a `TemplateContent` render fragment that lays out the wrapper markup.  The concrete implementation content is rendered within the grid as `this.Content`.  Note the cascading `ListController`, `BlazrPagingControl` and the Modal Dislog component.  

```csharp
@namespace Blazr.App.UI
@typeparam TRecord where TRecord : class, new()
@typeparam TEntityService where TEntityService : class, IEntityService
@inherits UIWrapperBase

@code {
    protected override RenderFragment TemplatedContent => (__builder) =>
    {
        <PageTitle>List of @this.UIEntityService.PluralDisplayName</PageTitle>

        <CascadingValue Value=this.Presenter.ListController IsFixed>
            <div class="container-fluid mb-2">
                <div class="row">
                    <div class="col-12 mb-2">
                        @this.TitleContent
                    </div>
                </div>
                <div class="row">
                    <div class="col-12 col-lg-8">
                        <BlazrPagingControl TRecord="TRecord" DefaultPageSize="20" />
                    </div>
                </div>
            </div>
            <BlazrGrid TGridItem="TRecord">
                @this.Content
            </BlazrGrid>

        </CascadingValue>
        <BaseModalDialog @ref=modalDialog />
    };

    protected virtual RenderFragment TitleContent => (__builder) =>
    {
        <h4>List of @this.UIEntityService.PluralDisplayName</h4>
    };
}
```



```csharp
public partial class UIPagedListFormBase<TRecord, TEntityService> : UIWrapperBase, IAsyncDisposable
    where TRecord : class, new()
    where TEntityService : class, IEntityService
{
    [Inject] protected IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] protected IUIEntityService<TEntityService> UIEntityService { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;

    [Parameter] public Guid StateId { get; set; } = Guid.Empty;

    public IListPresenter<TRecord, TEntityService> Presenter { get; set; } = default!;

    protected IModalDialog? modalDialog;

    private IDisposable? _disposable;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        // overries the base as we need to make sure we set up the Presenter Service before any rendering takes place
        parameters.SetParameterProperties(this);

        if (!initialized)
        {
            // Gets an instance of the Presenter from the Service Provider
            this.Presenter = ServiceProvider.GetComponentService<IListPresenter<TRecord, TEntityService>>() ?? default!;

            if (this.Presenter is null)
                throw new NullReferenceException($"No Presenter cound be created.");

            _disposable = this.Presenter as IDisposable;
            Presenter.StateId = this.StateId;
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected async Task OnEditAsync(TRecord record)
    {
        var id = RecordUtilities.GetIdentity(record);
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        if (modalDialog is not null && this.UIEntityService.EditForm is not null)
        {
            await modalDialog.ShowAsync(this.UIEntityService.EditForm, options);
            this.StateHasChanged();
        }
        else
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/edit/{id}");
    }

    protected async Task OnViewAsync(TRecord record)
    {
        var id = RecordUtilities.GetIdentity(record);
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        if (modalDialog is not null && this.UIEntityService.ViewForm is not null)
        {
            await modalDialog.ShowAsync(this.UIEntityService.ViewForm, options);
            this.StateHasChanged();
        }
        else
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/view/{id}");
    }

    public async ValueTask DisposeAsync()
    {
        _disposable?.Dispose();

        if (this.Presenter is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
    }
}
```

# List Contoller

List forms always turn out far most complex than a simple interation table.  They normally incorporate one or more of the following: paging, sorting, filtering and editing.

This design uses a DataGrid component that builds the table.  It's detail is covered in another note.

List operations revolve around a `ListController` that is registered with DI as a *Transient* service.

Here's the class outline.  It holds internally the data set as a List, and exposes the list by  implementing `IEnumerable<T>`.

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

Lets look at how paging works.  The paging control:

Registers itself with the controller.  Only one can be registered.
 
```csharp
public void RegisterPager(IListPagerProvider? pager) => _pagerProvider = pager;
```

And can then raise a paging event with the controller by calling `NotifyPagingRequestedAsync`.  The notifier calls `PagingRequestedAsync` on the registered consumer.

```csharp
public async ValueTask NotifyPagingRequestedAsync(object? sender, PagingEventArgs request)
{
    if (sender is null || sender != _pagerProvider)
    {
        _logger.LogError($"{sender?.GetType().Name} attempted to raise a Paging Event but is not the registered Pager Provider.");
        return;
    }

    if (_consumer is not null)
        await _consumer.PagingRequestedAsync(sender, request);
}
```

`PagingEventArgs` contains a `PagingRequest` and `InitialPageSize` which provides a mechanism for passing in a default page size into the consumer if the request is null.

```csharp
public sealed class PagingEventArgs : EventArgs
{
    public PagingRequest? Request { get; set; }
    public int? InitialPageSize { get; set; }
}
```

`PagingRequest` contains the basic paging data.

```csharp
public record PagingRequest
{
    public int PageSize { get; init; } = 1000;
    public int StartIndex { get; init; } = 0;
    public int Page => StartIndex <= 0 ? 0 : StartIndex / PageSize;
}
```
