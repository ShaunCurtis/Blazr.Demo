/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IFilteredListQuery<TRecord>
    : ICQSRequest<ValueTask<ListProviderResult<TRecord>>>
{
    public ListProviderRequest Request { get; }

    public Func<TRecord, bool>? FilterExpression { get; }
}
