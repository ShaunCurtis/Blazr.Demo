/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record DeleteRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
    where TRecord : class, new()
{
    public DeleteRecordCommand() { }

    public static DeleteRecordCommand<TRecord> GetCommand(TRecord record)
        => new() { Record = record };

    public static DeleteRecordCommand<TRecord> GetCommand(
        in APICommandProviderRequest<TRecord> request,
        CancellationToken? cancellationToken = null)
            => new()
            {
                TransactionId = request.TransactionId,
                Record = request.Record,
                CancellationToken = cancellationToken ?? new CancellationToken()
            };

}
