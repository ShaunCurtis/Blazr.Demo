/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public sealed record ListQueryAPIRequest
{
    public int StartIndex { get; init; } = 0;
    public int PageSize { get; init; } = 1000;
    public List<FilterDefinition>? Filters { get; init; }
    public List<SortDefinition>? Sorters { get; init; }

    public ListQueryAPIRequest() { }

    public static ListQueryAPIRequest FromRequest(ListQueryRequest request)
        => new()
        {
            StartIndex = request.StartIndex,
            PageSize = request.PageSize,
            Filters = request.Filters.ToList(),
            Sorters = request.Sorters.ToList(),
        };

    public ListQueryRequest ToRequest(CancellationToken? cancellation = null)
        => new()
        {
            Filters = this.Filters ?? Enumerable.Empty<FilterDefinition>(),
            Sorters = this.Sorters ?? Enumerable.Empty<SortDefinition>(),
            StartIndex = this.StartIndex,
            PageSize = this.PageSize,
            Cancellation = cancellation ?? CancellationToken.None
        };
}
