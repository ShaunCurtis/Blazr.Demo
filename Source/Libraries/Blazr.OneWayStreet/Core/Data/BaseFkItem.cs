/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.ComponentModel.DataAnnotations;

namespace Blazr.OneWayStreet.Core;

/// <summary>
/// The base IFkItem foreign key implementation 
/// </summary>
public record BaseFkItem 
    : IFkItem
{
    [Key]
    public Guid Uid { get; init; }

    public string Name { get; init; } = String.Empty;
}
