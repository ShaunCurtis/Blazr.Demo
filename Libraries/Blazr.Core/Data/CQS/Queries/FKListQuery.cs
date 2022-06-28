/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record FKListQuery<TRecord>
    : ICQSRequest<ValueTask<FKListProviderResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
}
