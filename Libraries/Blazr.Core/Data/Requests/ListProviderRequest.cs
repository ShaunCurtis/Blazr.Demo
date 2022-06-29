/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct ListProviderRequest
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public CancellationToken CancellationToken { get; }

    public string? SortExpression { get; }

    public string? FilterExpression { get; }

    public ItemsProviderRequest Request => new (this.StartIndex, this.PageSize, this.CancellationToken);

    public ListProviderRequest(int startIndex, int pageSize, CancellationToken cancellationToken, string? sortExpression = null, string? filterExpression = null )
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        CancellationToken = cancellationToken;
        SortExpression = sortExpression;
        FilterExpression = filterExpression;
    }

    public ListProviderRequest(ItemsProviderRequest request)
    {
        StartIndex = request.StartIndex;
        PageSize = request.Count;
        CancellationToken = request.CancellationToken;
        SortExpression = null;
        FilterExpression = null;
    }

    public ListProviderRequest(ListState options)
    {
        StartIndex = options.StartIndex;
        PageSize = options.PageSize;
        CancellationToken = new CancellationToken();
        SortExpression = null;
        FilterExpression = null;
    }
}
