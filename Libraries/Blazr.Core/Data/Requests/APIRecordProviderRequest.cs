/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct APIRecordProviderRequest<TRecord>
    where TRecord : class, new()
{
    public Guid TransactionId { get; }

    public Guid Uid { get; }

    private APIRecordProviderRequest(RecordQuery<TRecord> command)
    {
        TransactionId = command.TransactionId;
        Uid = command.Uid;
    }

    public static APIRecordProviderRequest<TRecord> GetRequest(RecordQuery<TRecord> query)
        => new APIRecordProviderRequest<TRecord>(query);
}
