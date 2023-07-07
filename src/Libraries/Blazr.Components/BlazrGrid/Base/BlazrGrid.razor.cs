/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components.BlazrGrid;

// The Component is configured to get it's data set from one of two sources (in order of precidence):
//   1. The Parameter provided ListComponentController.
//   2. The cascaded cascadedListComponentController instance.

public partial class BlazrGrid<TGridItem> : BlazrBaseComponent, IComponent, IHandleEvent, IDisposable
    where TGridItem : class, new()
{
    /// <summary>
    /// One of the three mechanisms for providing 
    /// </summary>
    [CascadingParameter] private IListController<TGridItem>? cascadedListComponentController { get; set; }

    [Parameter] public IListController<TGridItem>? ListComponentController { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private IEnumerable<TGridItem> _items = Enumerable.Empty<TGridItem>();

    private IListController<TGridItem>? _listComponentController { get; set; }
    protected readonly List<IBlazrGridColumn<TGridItem>> GridColumns = new();

    public void RegisterColumn(IBlazrGridColumn<TGridItem> column)
    {
        if (!this.GridColumns.Any(item => item.ComponentUid == column.ComponentUid))
            this.GridColumns.Add(column);
    }

    public async Task SetParametersAsync(ParameterView parameters)
    {
        // Sets the component parameters to the supplied values
        parameters.SetParameterProperties(this);

        // Set the list Controller, prioritizing the Parameter Value
        if (this.NotInitialized)
        {
            _listComponentController = ListComponentController ?? cascadedListComponentController;

            // Wires up the component to the controller's ListChanged event if it exists
            if (this._listComponentController is not null)
            {
                _listComponentController.StateChanged += OnListChanged;

                // If we don't have a pager we need to manually initialise the first data set query
                if (!_listComponentController.HasPager)
                {
                    DummyPager pager = new();
                    _listComponentController.RegisterPager(pager);
                    await _listComponentController.NotifyPagingRequestedAsync(pager, new PagingEventArgs { Request = new() });
                    _listComponentController.RegisterPager(null);
                }
            }

            // Render the component so all the columns can register
            await this.RenderAsync();
        }

        // Sets the internal TGridItem collection
        _items = _listComponentController ?? Enumerable.Empty<TGridItem>();

        // Render the grid content
        this.StateHasChanged();
    }

    public async ValueTask RefreshAsync()
        => await this.RenderAsync();

    public async void OnListChanged(object? sender, EventArgs e)
        => await this.RenderAsync();

    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? obj)
    {
        await item.InvokeAsync(obj);
        this.StateHasChanged();
    }

    public void Dispose()
    {
        if (this._listComponentController is not null)
        {
            _listComponentController.StateChanged -= OnListChanged;
        }
    }
}
