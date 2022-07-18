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
    private IUiStateService? _uiStateService;
    private ListState _currentRecord = default!;
    private Func<ListState, ValueTask<(int, bool)>>? _listProvider { get; set; }
    private string sortDirectionText => SortDescending ? " Desc" : string.Empty;

    public readonly Guid Id = Guid.NewGuid();

    public string? SortField { get; set; }

    public bool SortDescending { get; set; }

    public int PageSize { get; set; } = 1000;

    public int StartIndex { get; set; } = 0;

    public int ListTotalCount { get; set; } = 0;

    public int Page => StartIndex / PageSize;

    public bool IsDirty => this.ListStateRecord != _currentRecord;

    public string SortExpression => $"{SortField}{sortDirectionText}";

    public ListState ListStateRecord => new ListState()
    {
        PageSize = this.PageSize,
        StartIndex = StartIndex,
        SortField = SortField,
        SortDescending = SortDescending,
        SortExpression = this.SortExpression,
        ListTotalCount = this.ListTotalCount
    };

    public PagingRequest PagingState => new PagingRequest
    {
        PageSize = PageSize,
        StartIndex = StartIndex,
    };

    public event EventHandler<PagingEventArgs>? PagingReset;

    internal void Load(Guid stateId, Func<ListState, ValueTask<(int, bool)>>? listProvider)
    {
        _stateId = stateId;
        _listProvider = listProvider;
        _hasLoaded = true;

        this.GetState();
    }

    public bool Set(ListState? state)
    {
        if (state != null)
        {
            _currentRecord = state with { };
            this.PageSize = state.PageSize;
            this.StartIndex = state.StartIndex;
            this.SortField = state.SortField;
            this.SortDescending = state.SortDescending;
            this.ListTotalCount = state.ListTotalCount;
        }
        return state != null;
    }

    public void Set(PagingRequest? request)
    {
        if (request is not null)
        {
            this.StartIndex = request.StartIndex;
            this.PageSize = request.PageSize;
        }
    }

    public ListContext(IUiStateService uiStateService)
    {
        _uiStateService = uiStateService;
    }

    public async ValueTask<bool> PageAsync(PagingRequest? request = null)
    {
        if (!_hasLoaded)
            throw new InvalidOperationException("You can't use the ListContext untill you have loaded it.");

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

    public async ValueTask SortAsync(SortRequest request)
    {
        if (!_hasLoaded)
            throw new InvalidOperationException("You can't use the ListContext untill you have loaded it.");

        var isPagingReset = !request.SortField?.Equals(this.SortField);

        this.GetState();

        // If we are sorting on a new field then we need to reset the page
        if (isPagingReset ??= false)
            this.StartIndex = 0;

        this.SortField = request.SortField;
        this.SortDescending = request.SortDescending;

        if (_listProvider is not null)
        {
            var returnState = await _listProvider(this.ListStateRecord);

            if (returnState.Item2)
                this.ListTotalCount = returnState.Item1;
        }

        this.SaveState();

        if (isPagingReset ??= false)
            PagingReset?.Invoke(this, new PagingEventArgs(this.PagingState));
    }

    public bool GetState()
    {
        var result =  _uiStateService?.TryGetStateData<ListState>(_stateId, out ListState? state) ?? false
             ? this.Set(state)
             : false;

        _currentRecord = this.ListStateRecord;
        return result;
    }

    public bool CheckState()
        => _uiStateService?.TryGetStateData<ListState>(_stateId, out ListState? state) ?? false;

    public void SaveState()
    {
        _uiStateService?.AddStateData(_stateId, this.ListStateRecord);
        _currentRecord = this.ListStateRecord;
    }
}
