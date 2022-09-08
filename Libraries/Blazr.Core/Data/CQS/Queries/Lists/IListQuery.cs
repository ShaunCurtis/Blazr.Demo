
using System.Linq.Expressions;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IListQuery<TRecord>
    : IRequest<ValueTask<ListProviderResult<TRecord>>>
    where TRecord : class, new()
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public bool SortDescending { get; }

    public Expression<Func<TRecord, bool>>? FilterExpression { get; }

    public Expression<Func<TRecord, object>>? SortExpression { get; }

    public CancellationToken CancellationToken { get; }
}
