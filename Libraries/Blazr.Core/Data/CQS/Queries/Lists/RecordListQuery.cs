/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record RecordListQuery<TRecord>
    : ICQSRequest<ValueTask<ListProviderResult<TRecord>>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    
    public ListProviderRequest Request { get; protected set; }

    public RecordListQuery(ListProviderRequest request)
        => Request = request;
}
