/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record SortState
{
    public string? SortField { get; init; }

    public bool SortDescending { get; init; }

    public bool IsSorting => !string.IsNullOrWhiteSpace(SortField);

    public static SortState GetSortState(ListState options)
        =>  new SortState { SortDescending = options.SortDescending, SortField=options.SortField  };
}
