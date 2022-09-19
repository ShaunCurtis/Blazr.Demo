/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct APIFKListQueryProviderRequest<TRecord>
    where TRecord : class, IFkListItem, new()
{
    public Guid TransactionId { get; }

    private APIFKListQueryProviderRequest(FKListQuery<TRecord> query)
        => TransactionId = query.TransactionId;

    public static APIFKListQueryProviderRequest<TRecord> GetRequest(FKListQuery<TRecord> query)
        => new APIFKListQueryProviderRequest<TRecord>(query);
}
