/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record ListState
{
    public string? SortField { get; init; }

    public bool SortDescending { get; init; }

    public int PageSize { get; init; } = 1000;

    public int StartIndex { get; init; } = 0;

    public int ListTotalCount { get; init; } = 0;

    public string? FilterExpression { get; init; }

    public int Page => StartIndex / PageSize;

    public string? SortExpression()
    {
        string _sortDescendingText = this.SortDescending ? "Desc" : string.Empty;

        return SortField is not null
            ? $"{SortField} {_sortDescendingText}".Trim()
            : null;
    }
}
