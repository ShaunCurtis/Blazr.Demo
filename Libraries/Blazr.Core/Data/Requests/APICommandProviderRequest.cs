/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct APICommandProviderRequest<TRecord>
    where TRecord : class, new()
{
    public Guid TransactionId { get; }

    public TRecord Record { get; }

    private APICommandProviderRequest(IRecordCommand<TRecord> command)
    {
        TransactionId = command.TransactionId;
        Record = command.Record;
    }

    public static APICommandProviderRequest<TRecord> GetRequest(IRecordCommand<TRecord> command)
        => new APICommandProviderRequest<TRecord>(command);
}
