
using Serialize.Linq.Serializers;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct APIListProviderRequest<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; init; }

    public int PageSize { get; init;  }

    public bool SortDescending { get; init; }

    public string? SortExpressionString { get; init;  }

    public string? FilterExpressionString { get; init;  }

    private APIListProviderRequest(IListQuery<TRecord> query)
    {
        StartIndex = query.StartIndex;
        PageSize = query.PageSize;
        SortDescending = query.SortDescending;
        SortExpressionString = SerializeSorter(query.SortExpression);
        FilterExpressionString = SerializeFilter(query.FilterExpression);
    }

    private static string? SerializeFilter(Expression<Func<TRecord, bool>>? filter)
    {
        if (filter is not null)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            return serializer.SerializeText(filter);
        }

        return null;
    }

    private static string? SerializeSorter(Expression<Func<TRecord, object>>? sorter)
    {
        if (sorter is not null)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            return serializer.SerializeText(sorter);
        }

        return null;
    }
}
