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

    public string? SortField { get; }

    public bool SortDescending { get; }

    public CancellationToken CancellationToken { get; }

    public string? SortExpressionString { get; }

    public string? FilterExpressionString { get; }

    public Func<TRecord, bool>? FilterExpression { get; }

    public ItemsProviderRequest Request => new (this.StartIndex, this.PageSize, this.CancellationToken);

    public ListProviderRequest()
    {
        StartIndex = 0;
        PageSize = 10000;
        CancellationToken = new CancellationToken();
        SortExpressionString = null;
        FilterExpressionString = null;
        FilterExpression = null;
        SortField = null;
        SortDescending = false;
    }

    public ListProviderRequest(int startIndex, int pageSize, CancellationToken cancellationToken, string? sortExpressionString = null, string? filterExpressionString = null, Func<TRecord, bool>? filterExpression = null )
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        CancellationToken = cancellationToken;
        SortExpressionString = sortExpressionString;
        FilterExpressionString = filterExpressionString;
        FilterExpression = filterExpression;
        SortField = null;
        SortDescending = false;
    }

    public ListProviderRequest(ItemsProviderRequest request, Func<TRecord, bool>? filterExpression = null)
    {
        StartIndex = request.StartIndex;
        PageSize = request.Count;
        CancellationToken = request.CancellationToken;
        SortExpressionString = null;
        FilterExpressionString = null;
        FilterExpression = filterExpression;
        SortField = null;
        SortDescending = false;
    }

    public ListProviderRequest(ListState options, Func<TRecord, bool>? filterExpression = null)
    {
        StartIndex = options.StartIndex;
        PageSize = options.PageSize;
        CancellationToken = new CancellationToken();
        SortDescending = options.SortDescending;
        SortField = options.SortField;
        SortExpressionString = null;
        FilterExpressionString = null;
        FilterExpression = filterExpression;
    }
}
