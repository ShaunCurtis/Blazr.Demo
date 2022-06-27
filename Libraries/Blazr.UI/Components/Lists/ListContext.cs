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

    public readonly ListState ListState = new();

    private Func<ListState, ValueTask<ListState>>? _listProvider { get; set; }

    public event EventHandler<PagingEventArgs>? PagingReset; 

    public ListContext() { }

    public ListContext(ListState options)
        => ListState.Load(options);

    internal void Attach(UiStateService? uiStateService, Guid stateId, Func<ListState, ValueTask<ListState>>? listProvider)
    {
        _uiStateService = uiStateService;
        _stateId = stateId;
        _listProvider = listProvider;
        _hasLoaded = false;

        if (_uiStateService is not null)
            this.GetState();
    }

    public async ValueTask<PagingState> SetPage(PagingState pagingStatr)
    {
        var hasNoState = !this.GetState();
        if (_hasLoaded || hasNoState)
        {
            this.ListState.StartIndex = pagingStatr.StartIndex;
            this.ListState.PageSize = pagingStatr.PageSize;
        }
        if (_listProvider is not null)
        {
            var returnOptions = await _listProvider(this.ListState);
            if (returnOptions != null)
                this.ListState.ListTotalCount = returnOptions.ListTotalCount;
        }
        _hasLoaded = true;
        this.SaveState();
        return this.ListState.PagingState;
    }

    public async ValueTask SetSortState(SortState sortState)
    {
        var isPagingReset = !sortState.SortField?.Equals(this.ListState.SortState.SortField);

        this.GetState();

        // If we are sorting on a new field then we need to reset the page
        if (isPagingReset ??= false)
            this.ListState.StartIndex = 0;

        this.ListState.SortField = sortState.SortField;
        this.ListState.SortDescending = sortState.SortDescending;
        if (_listProvider is not null)
        {
            var returnState = await _listProvider(this.ListState);
            if (returnState != null)
                this.ListState.ListTotalCount = returnState.ListTotalCount;
        }
        this.SaveState();
        if (isPagingReset ??= false )
            PagingReset?.Invoke(this, new PagingEventArgs(ListState.PagingState));
    }

    private bool GetState()
    {
        if (_stateId != Guid.Empty && _uiStateService is not null && _uiStateService.TryGetStateData<ListState>(_stateId, out object? stateOptions) && stateOptions is ListState)
        {
            this.ListState.Load((ListState)stateOptions);
            return true;
        }
        return false;
    }

    private void SaveState()
    {
        if (_stateId != Guid.Empty && _uiStateService is not null)
            _uiStateService.AddStateData(_stateId, this.ListState);
    }
}
