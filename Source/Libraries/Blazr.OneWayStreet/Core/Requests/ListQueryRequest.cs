/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public sealed record ListQueryRequest
{
    public int StartIndex { get; init; } = 0;
    public int PageSize { get; init; } = 1000;
    public CancellationToken Cancellation { get; set; } = new();
    public IEnumerable<FilterDefinition> Filters { get; init; } = Enumerable.Empty<FilterDefinition>();
    public IEnumerable<SortDefinition> Sorters { get; init; } = Enumerable.Empty<SortDefinition>();
}
