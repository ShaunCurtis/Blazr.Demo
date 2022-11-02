/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;
using System.ComponentModel.DataAnnotations;

public record DboIdentity
{
    [Key]
    public Guid Id { get; init; } = Guid.Empty;

    public string Name { get; init; } = String.Empty;

    public string Role { get; init; } = String.Empty;
}
