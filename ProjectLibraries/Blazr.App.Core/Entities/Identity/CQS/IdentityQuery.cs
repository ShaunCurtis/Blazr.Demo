/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record IdentityQuery
    : IRequestAsync<ValueTask<IdentityRequestResult>>
{
    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public CancellationToken CancellationToken { get; init; } = default;

    public Guid IdentityId { get; init; } = Guid.Empty;

    public static IdentityQuery GetQuery(Guid Uid)
        => new IdentityQuery { IdentityId = Uid };

    public static IdentityQuery GetQuery(APIIdentityProviderRequest request, CancellationToken cancellationToken = default)
        => new IdentityQuery { TransactionId = request.TransactionId, CancellationToken = cancellationToken };

}
