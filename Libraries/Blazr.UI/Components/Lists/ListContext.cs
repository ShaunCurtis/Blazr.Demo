/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ListContext
{
    private Guid _stateId = Guid.Empty;
    private bool _hasLoaded;
    private UiStateService? _uiStateService;

    public readonly ListOptions ListOptions = new();

    private Func<ListOptions, ValueTask<ListOptions>>? _listProvider { get; set; }

    public event EventHandler<PagingEventArgs>? PagingReset; 

    public ListContext() { }

    public ListContext(ListOptions options)
        => ListOptions.Load(options);

    internal void Attach(UiStateService? uiStateService, Guid stateId, Func<ListOptions, ValueTask<ListOptions>>? listProvider)
    {
        _uiStateService = uiStateService;
        _stateId = stateId;
        _listProvider = listProvider;
        _hasLoaded = false;

        if (_uiStateService is not null)
            this.GetState();
    }

    public async ValueTask<PagingOptions> SetPage(PagingOptions pagingOptions)
    {
        var hasNoState = !this.GetState();
        if (_hasLoaded || hasNoState)
        {
            this.ListOptions.StartIndex = pagingOptions.StartIndex;
            this.ListOptions.PageSize = pagingOptions.PageSize;
        }
        if (_listProvider is not null)
        {
            var returnOptions = await _listProvider(this.ListOptions);
            if (returnOptions != null)
                this.ListOptions.ListTotalCount = returnOptions.ListTotalCount;
        }
        _hasLoaded = true;
        this.SaveState();
        return this.ListOptions.PagingOptions;
    }

    public async ValueTask SetSortState(SortOptions sortOptions)
    {
        var isPagingReset = !sortOptions.SortField?.Equals(this.ListOptions.SortOptions.SortField);

        this.GetState();

        // If we are sorting on a new field then we need to reset the page
        if (isPagingReset ??= false)
            this.ListOptions.StartIndex = 0;

        this.ListOptions.SortField = sortOptions.SortField;
        this.ListOptions.SortDescending = sortOptions.SortDescending;
        if (_listProvider is not null)
        {
            var returnOptions = await _listProvider(this.ListOptions);
            if (returnOptions != null)
                this.ListOptions.ListTotalCount = returnOptions.ListTotalCount;
        }
        this.SaveState();
        if (isPagingReset ??= false )
            PagingReset?.Invoke(this, new PagingEventArgs(ListOptions.PagingOptions));
    }

    private bool GetState()
    {
        if (_stateId != Guid.Empty && _uiStateService is not null && _uiStateService.TryGetStateData<ListOptions>(_stateId, out object? stateOptions) && stateOptions is ListOptions)
        {
            this.ListOptions.Load((ListOptions)stateOptions);
            return true;
        }
        return false;
    }

    private void SaveState()
    {
        if (_stateId != Guid.Empty && _uiStateService is not null)
            _uiStateService.AddStateData(_stateId, this.ListOptions);
    }
}
