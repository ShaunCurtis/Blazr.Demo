
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class IdentityQuery
    : IRequest<ValueTask<IdentityQueryResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();

    public Guid IdentityId { get; set; }
}

public record IdentityQueryResult
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public ClaimsPrincipal Identity { get; init; } = new ClaimsPrincipal();
}
