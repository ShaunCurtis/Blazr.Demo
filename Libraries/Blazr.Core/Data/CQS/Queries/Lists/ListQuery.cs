/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record ListQuery<TRecord>
    :ListQueryBase<TRecord>
    where TRecord : class, new()
{
    public ListQuery() { }

    public ListQuery(ListProviderRequest<TRecord> request)
        :base(request) { }
}
