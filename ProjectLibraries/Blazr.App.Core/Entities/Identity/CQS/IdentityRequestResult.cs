/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record IdentityRequestResult
{
    public ClaimsIdentity? Identity { get; init; } = null;

    public bool Success { get; init; } = false;

    public string Message { get; init; } = string.Empty;

    public static IdentityRequestResult Failure(string message)
        => new IdentityRequestResult {Message = message };

    public static IdentityRequestResult Successful(ClaimsIdentity identity, string? message = null)
        => new IdentityRequestResult {Identity = identity, Success=true, Message = message ?? string.Empty };
}

