/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record AddRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
    where TRecord : class, new()
{
    private AddRecordCommand() { }

    public static AddRecordCommand<TRecord> GetCommand(TRecord record)
        => new() { Record = record };

    public static AddRecordCommand<TRecord> GetCommand(
        in APICommandProviderRequest<TRecord> request,
        CancellationToken? cancellationToken = null)
           => new()
           {
               TransactionId = request.TransactionId,
               Record = request.Record,
               CancellationToken = cancellationToken ?? new CancellationToken()
           };
}
