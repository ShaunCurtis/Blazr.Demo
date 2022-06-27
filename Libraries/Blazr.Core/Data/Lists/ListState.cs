/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class ListState
{
    public string? SortField { get; set; }

    public bool SortDescending { get; set; }

    public int PageSize { get; set; } = 1000;

    public int StartIndex { get; set; } = 0;

    public int ListTotalCount { get; set; } = 0;

    public bool IsPaging => (PageSize > 0);

    public void Set(ListState state)
    {
        this.PageSize = state.PageSize;
        this.StartIndex = state.StartIndex;
        this.SortField = state.SortField;
        this.SortDescending = state.SortDescending;
    }

    public void Set(ItemsProviderRequest options)
    {
        this.PageSize = options.Count;
        this.StartIndex = options.StartIndex;
    }

    public int Page
        => StartIndex / PageSize;

    public PagingState PagingState => new PagingState
    {
        PageSize = PageSize,
        StartIndex = StartIndex,
        ListTotalCount = ListTotalCount
    };

    public SortState SortState => SortState.GetSortState(this);

    public ListState GetCopy(int listcount)
    {
        var copy = this.Copy;
        copy.ListTotalCount = listcount;
        return copy;
    }

    public ListState Copy
        => new ListState()
        {
            ListTotalCount = this.ListTotalCount,
            SortDescending = this.SortDescending,
            SortField = this.SortField,
            PageSize = this.PageSize,
            StartIndex = this.StartIndex
        };

    public void Load(ListState? state)
    {
        if (state is null)
            return;

        this.ListTotalCount = state.ListTotalCount;
        this.SortField = state.SortField;
        this.SortDescending = state.SortDescending;
        this.PageSize = state.PageSize;
        this.StartIndex = state.StartIndex;
    }
}
