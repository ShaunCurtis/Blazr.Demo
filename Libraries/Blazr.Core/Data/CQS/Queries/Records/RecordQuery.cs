/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record RecordQuery<TRecord>
    : ICQSRequest<ValueTask<RecordProviderResult<TRecord>>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();

    public readonly Guid GuidId;
    public readonly int IntId;
    public readonly long LongId;

    public RecordQuery(Guid recordId)
        => this.GuidId = recordId;

    public RecordQuery(int recordId)
        => this.IntId = recordId;

    public RecordQuery(long recordId)
        => this.LongId = recordId;
}
