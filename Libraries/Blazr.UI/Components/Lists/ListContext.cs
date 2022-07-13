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

    public readonly Guid Id = Guid.NewGuid();
    public readonly ListState ListState = new();

    private Func<ListStateRecord, ValueTask<(int, bool)>>? _listProvider { get; set; }

    public event EventHandler<PagingEventArgs>? PagingReset;

    public ListContext(UiStateService uiStateService)
    {
        _uiStateService = uiStateService;
    }

    internal void Load(Guid stateId, Func<ListStateRecord, ValueTask<(int, bool)>>? listProvider)
    {
        _stateId = stateId;
        _listProvider = listProvider;
        _hasLoaded = true;

        this.GetState();
    }

    public async ValueTask<bool> GoToPage(int? page = null)
    {
        if (!_hasLoaded)
            throw new InvalidOperationException("You can't use the ListContext untill you have loaded it.");

        this.ListState.Set(page);

        if (_listProvider is not null)
        {
            var result = await _listProvider(this.ListState.Record);
            if (result.Item2)
                this.ListState.ListTotalCount = result.Item1;

            this.SaveState();

            return result.Item2;
        }

        return false;
    }

    public async ValueTask SetSortState(SortState sortState)
    {
        if (!_hasLoaded)
            throw new InvalidOperationException("You can't use the ListContext untill you have loaded it.");

        var isPagingReset = !sortState.SortField?.Equals(this.ListState.SortState.SortField);

        this.GetState();

        // If we are sorting on a new field then we need to reset the page
        if (isPagingReset ??= false)
            this.ListState.StartIndex = 0;

        this.ListState.SortField = sortState.SortField;
        this.ListState.SortDescending = sortState.SortDescending;

        if (_listProvider is not null)
        {
            var returnState = await _listProvider(this.ListState.Record);

            if (returnState.Item2)
                this.ListState.ListTotalCount = returnState.Item1;
        }

        this.SaveState();

        if (isPagingReset ??= false)
            PagingReset?.Invoke(this, new PagingEventArgs(ListState.PagingState));
    }

    public bool GetState()
    {
        return _uiStateService?.TryGetStateData<ListStateRecord>(_stateId, out ListStateRecord? state) ?? false
             ? this.ListState.Set(state)
             : false;
    }

    public bool CheckState()
        => _uiStateService?.TryGetStateData<ListStateRecord>(_stateId, out ListStateRecord? state) ?? false;

    public void SaveState()
        => _uiStateService?.AddStateData(_stateId, this.ListState.Record);
}
