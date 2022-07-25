/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct ListProviderRequest<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public CancellationToken CancellationToken { get; }

    public string? SortExpressionString { get; }

    public string? FilterExpressionString { get; }

    public ItemsProviderRequest Request => new (this.StartIndex, this.PageSize, this.CancellationToken);

    public ListProviderRequest()
    {
        StartIndex = 0;
        PageSize = 10000;
        CancellationToken = new CancellationToken();
        SortExpressionString = null;
        FilterExpressionString = null;
    }
    public ListProviderRequest(int startIndex, int pageSize)
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        CancellationToken = new CancellationToken();
        SortExpressionString = null;
        FilterExpressionString = null;
    }

    public ListProviderRequest(int startIndex, int pageSize, CancellationToken cancellationToken, string? sortExpressionString = null, string? filterExpressionString = null)
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        CancellationToken = cancellationToken;
        SortExpressionString = sortExpressionString;
        FilterExpressionString = filterExpressionString;
    }

    public ListProviderRequest(ItemsProviderRequest request, string? filterExpressionString = null)
    {
        StartIndex = request.StartIndex;
        PageSize = request.Count;
        CancellationToken = request.CancellationToken;
        SortExpressionString = null;
        FilterExpressionString = filterExpressionString;
    }

    public ListProviderRequest(ListState options, string? filterExpressionString = null)
    {
        StartIndex = options.StartIndex;
        PageSize = options.PageSize;
        CancellationToken = new CancellationToken();
        SortExpressionString = options.SortExpression;
        FilterExpressionString = filterExpressionString;
    }
}
