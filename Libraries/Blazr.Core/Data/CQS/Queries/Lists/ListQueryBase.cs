/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public abstract record ListQueryBase<TRecord>
    : IListQuery<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; protected init; }

    public int PageSize { get; protected init; }

    public bool SortDescending { get; protected init; }

    public Expression<Func<TRecord, bool>>? FilterExpression { get; protected init; }

    public Expression<Func<TRecord, object>>? SortExpression { get; protected init; }

    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public CancellationToken CancellationToken { get; protected init; } = default;

    protected ListQueryBase() { }

    protected ListQueryBase(in ListProviderRequest<TRecord> request)
    {
        this.StartIndex = request.StartIndex;
        this.PageSize = request.PageSize;
        this.SortDescending = request.SortDescending;
        this.SortExpression = request.SortExpression;
        this.FilterExpression = request.FilterExpression;
        this.CancellationToken = request.CancellationToken;
    }

    protected ListQueryBase(in APIListProviderRequest<TRecord> request, CancellationToken? cancellationToken = null)
    {
        this.TransactionId = request.TransactionId;
        this.StartIndex = request.StartIndex;
        this.PageSize = request.PageSize;
        this.SortDescending = request.SortDescending;
        this.SortExpression = DeSerializeSorter(request.SortExpressionString);
        this.FilterExpression = DeSerializeFilter(request.FilterExpressionString);
        this.CancellationToken = cancellationToken ?? new CancellationToken();
    }

    protected static Expression<Func<TRecord, bool>>? DeSerializeFilter(string? filter)
    {
        if (filter is null)
            return null;

        var serializer = new ExpressionSerializer(new JsonSerializer());
        return (Expression<Func<TRecord, bool>>)serializer.DeserializeText(filter);
    }

    protected static Expression<Func<TRecord, object>>? DeSerializeSorter(string? sorter)
    {
        if (sorter is null)
            return null;

        var serializer = new ExpressionSerializer(new JsonSerializer());
        return (Expression<Func<TRecord, object>>)serializer.DeserializeText(sorter);
    }
}
