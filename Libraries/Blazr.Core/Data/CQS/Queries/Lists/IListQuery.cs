/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IListQuery<TRecord>
    : ICQSRequest<ValueTask<ListProviderResult<TRecord>>>
    where TRecord : class, new()
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public string? SortExpressionString { get; }

    public string? FilterExpressionString { get; }

    public CancellationToken CancellationToken { get; }
}
