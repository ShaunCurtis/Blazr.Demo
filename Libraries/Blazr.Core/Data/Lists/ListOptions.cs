/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class ListOptions
{
    public string? SortField { get; set; }

    public bool SortDescending { get; set; }

    public int PageSize { get; set; } = 1000;

    public int StartIndex { get; set; } = 0;

    public int ListTotalCount { get; set; } = 0;

    public bool IsPaging => (PageSize > 0);

    public void Set(ListOptions options)
    {
        this.PageSize = options.PageSize;
        this.StartIndex = options.StartIndex;
        this.SortField = options.SortField;
        this.SortDescending = options.SortDescending;
    }

    public void Set(ItemsProviderRequest options)
    {
        this.PageSize = options.Count;
        this.StartIndex = options.StartIndex;
    }

    public int Page
        => StartIndex / PageSize;

    public PagingOptions PagingOptions => new PagingOptions
    {
        PageSize = PageSize,
        StartIndex = StartIndex,
        ListTotalCount = ListTotalCount
    };

    public SortOptions SortOptions => SortOptions.GetSortOptions(this);

    public ListOptions GetCopy(int listcount)
    {
        var copy = this.Copy;
        copy.ListTotalCount = listcount;
        return copy;
    }

    public ListOptions Copy
        => new ListOptions()
        {
            ListTotalCount = this.ListTotalCount,
            SortDescending = this.SortDescending,
            SortField = this.SortField,
            PageSize = this.PageSize,
            StartIndex = this.StartIndex
        };

    public void Load(ListOptions? options)
    {
        if (options is null)
            return;

        this.ListTotalCount = options.ListTotalCount;
        this.SortField = options.SortField;
        this.SortDescending = options.SortDescending;
        this.PageSize = options.PageSize;
        this.StartIndex = options.StartIndex;
    }
}
