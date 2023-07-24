/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;
public readonly record struct UserUid(Guid Value);

public sealed record User : IIdentity
{
    public UserUid UserUid { get; init; }

    public EntityState EntityState { get; init; }

    public string UserName { get; init; } = "Not Set";

    public string Roles { get; init; } = "Not Set";

    public EntityUid Uid => new(UserUid.Value);
}
