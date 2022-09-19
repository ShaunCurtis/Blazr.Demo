/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record ListQuery<TRecord>
    :ListQueryBase<TRecord>
    where TRecord : class, new()
{
    private ListQuery(ListProviderRequest<TRecord> request)
    : base(request) { }
    
    private ListQuery(APIListProviderRequest<TRecord> request, CancellationToken? cancellationToken = null)
    : base(request, cancellationToken) { }

    public static ListQuery<TRecord> GetQuery(ListProviderRequest<TRecord> request)
        => new ListQuery<TRecord>(request);
 
    public static ListQuery<TRecord> GetQuery(in APIListProviderRequest<TRecord> request, CancellationToken cancellationToken = default)
        => new ListQuery<TRecord>(request, cancellationToken);
}
