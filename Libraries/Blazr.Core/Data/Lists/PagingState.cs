/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record PagingState
{
    public int PageSize { get; init; } = 1000;

    public int StartIndex { get; init; } = 0;

    public int ListTotalCount { get; init; } = 0;

    public int Page => StartIndex <= 0
        ? 0
        : StartIndex / PageSize;
}
