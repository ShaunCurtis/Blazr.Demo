/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record PagingOptions
{
    public int PageSize { get; init; } = 1000;

    public int StartRecord { get; init; } = 0;

    public int TotalListCount { get; init; } = 0;

    public int Page => StartRecord / PageSize;
}
