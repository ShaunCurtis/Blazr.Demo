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
    public int StartIndex { get; init; }

    public int PageSize { get; init; }

    public bool SortDescending { get; init; }

    public Expression<Func<TRecord, bool>>? FilterExpression { get; init; }

    public Expression<Func<TRecord, object>>? SortExpression { get; init; }

    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public CancellationToken CancellationToken { get; }

    public ListQueryBase()
        => this.CancellationToken = new CancellationToken();

    public ListQueryBase(ListProviderRequest<TRecord> request)
    {
        this.StartIndex = request.StartIndex;
        this.PageSize = request.PageSize;
        this.SortExpression = request.SortExpression;
        this.FilterExpression = request.FilterExpression;
        this.CancellationToken = request.CancellationToken;
    }

    public ListQueryBase(APIListProviderRequest<TRecord> request)
    {
        this.StartIndex = request.StartIndex;
        this.PageSize = request.PageSize;
        this.SortExpression = DeSerializeSorter(request.SortExpressionString);
        this.FilterExpression = DeSerializeFilter(request.FilterExpressionString);
    }

    protected static Expression<Func<TRecord, bool>>? DeSerializeFilter(string? filter)
    {
        if (filter is not null)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            return (Expression<Func<TRecord, bool>>)serializer.DeserializeText(filter);
        }

        return null;
    }

    protected static Expression<Func<TRecord, object>>? DeSerializeSorter(string? sorter)
    {
        if (sorter is not null)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            return (Expression<Func<TRecord, object>>)serializer.DeserializeText(sorter);
        }

        return null;
    }
}
