/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record ProductFkItem
    : BaseFkItem
{
    public string ProductCode { get; init; } = string.Empty; 
    public decimal ItemUnitPrice { get; init; }
}
