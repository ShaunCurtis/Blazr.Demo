/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record LookupListQuery<TRecord>
    : IHandlerRequest<ValueTask<LookupListProviderResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
}
