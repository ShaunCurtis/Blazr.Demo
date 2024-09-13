/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.OneWayStreet.Core;

public interface ILongLookUpItem
{
    long Id { get; }
    string Name { get; }
}

public record BaseLongLookUpItem : ILongLookUpItem
{
    [Key] public long Id { get; init; }

    public string Name { get; init; } = string.Empty;
}

