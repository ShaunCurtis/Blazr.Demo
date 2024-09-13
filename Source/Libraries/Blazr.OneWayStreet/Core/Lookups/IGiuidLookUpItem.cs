/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.OneWayStreet.Core;

public interface IGuidLookUpItem
{
    Guid Id { get; }
    string Name { get; }
}

public record BaseGuidLookUpItem : IGuidLookUpItem
{
    [Key] public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
}

