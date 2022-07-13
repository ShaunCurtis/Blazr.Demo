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

    public string SortExpression { get; init; } = string.Empty;
}
