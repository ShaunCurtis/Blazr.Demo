/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public record class ListQueryRequest
{
    public int StartIndex { get; init; }
    public int PageSize { get; init; }
    public CancellationToken Cancellation { get; init; }
    public IEnumerable<FilterDefinition> Filters { get; init; }
    public IEnumerable<SortDefinition> Sorters { get; init; }

    public ListQueryRequest()
    {
        StartIndex = 0;
        PageSize = 1000;
        Cancellation = new();
        Filters = Enumerable.Empty<FilterDefinition>();
        Sorters = Enumerable.Empty<SortDefinition>();
    }
}
