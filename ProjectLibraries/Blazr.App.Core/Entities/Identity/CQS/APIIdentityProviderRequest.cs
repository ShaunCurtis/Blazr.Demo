/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly struct APIIdentityProviderRequest
{
    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public Guid IdentityId { get; init; } = Guid.Empty;

    private APIIdentityProviderRequest(IdentityQuery query)
        => IdentityId = query.IdentityId;

    public static APIIdentityProviderRequest GetRequest(IdentityQuery query)
        => new APIIdentityProviderRequest(query);
}
