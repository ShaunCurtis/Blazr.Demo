/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Core;

public sealed record User : IGuidIdentity
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;

    public string UserName { get; init; } = "Not Set";

    public string Roles { get; init; } = "Not Set";
}
