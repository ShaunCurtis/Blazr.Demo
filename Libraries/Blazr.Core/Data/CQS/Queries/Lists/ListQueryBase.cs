/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public abstract record ListQueryBase<TRecord>
    :IListQuery<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; init; }

    public int PageSize { get; init; }

    public string? SortExpressionString { get; init; }

    public string? FilterExpressionString { get; init; }

    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public ListQueryBase() { }

    public ListQueryBase(ListProviderRequest<TRecord> request)
    {
        this.StartIndex = request.StartIndex;
        this.PageSize = request.PageSize;
        this.SortExpressionString = request.SortExpressionString;
        this.FilterExpressionString = request.FilterExpressionString;
    }
}
