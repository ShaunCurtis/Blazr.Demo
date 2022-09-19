/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record RecordQuery<TRecord>
    : IRequestAsync<ValueTask<RecordProviderResult<TRecord>>>
    where TRecord : class, new()
{
    public Guid TransactionId { get; private init; } = Guid.NewGuid();

    public Guid Uid { get; private init; }

    public CancellationToken CancellationToken { get; init; } = default;

    private RecordQuery() { }

    public static RecordQuery<TRecord> GetQuery(Guid recordId)
        => new() { Uid = recordId };

    public static RecordQuery<TRecord> GetQuery(
        APIRecordProviderRequest<TRecord> request,
        CancellationToken? cancellationToken = default)
            => new()
            {
                TransactionId = request.TransactionId,
                Uid = request.Uid,
                CancellationToken = cancellationToken ?? new CancellationToken()
            };
}
