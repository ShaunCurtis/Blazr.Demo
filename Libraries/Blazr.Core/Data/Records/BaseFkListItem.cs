﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.ComponentModel.DataAnnotations;

namespace Blazr.Core;

public record BaseFkListItem 
    : IFkListItem
{
    [Key]
    public Guid Id { get; init; }

    public string Name { get; init; } = String.Empty;
}
