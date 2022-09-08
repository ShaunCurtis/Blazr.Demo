/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Serialize.Linq.Serializers;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Blazr.Core;

public readonly struct ListProviderRequest<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public CancellationToken CancellationToken { get; }

    public Expression<Func<TRecord, bool>>? FilterExpression { get; init; }

    public Expression<Func<TRecord, object>>? SortExpression { get; init; }

    public ItemsProviderRequest Request => new (this.StartIndex, this.PageSize, this.CancellationToken);

    public ListProviderRequest()
    {
        StartIndex = 0;
        PageSize = 10000;
        CancellationToken = new CancellationToken();
        SortExpression = null;
        FilterExpression = null;
    }

    public ListProviderRequest(ListState<TRecord> options, Expression<Func<TRecord, bool>>? filter = null)
    {
        StartIndex = options.StartIndex;
        PageSize = options.PageSize;
        CancellationToken = new CancellationToken();
        SortExpression = options.SortExpression;
        FilterExpression = options. SerializeFilter(filter);
    }

    public static string? SerializeFilter(Expression<Func<TRecord, bool>>? filter)
    {
        if (filter is not null)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            return serializer.SerializeText(filter);
        }

        return null;
    }
}
