/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct FKListProviderResult
{
    public IEnumerable<IFkListItem> Items { get; }

    public bool Success { get; }

    public string? Message { get; }

    public FKListProviderResult(IEnumerable<IFkListItem> items, bool success = true, string? message = null)
    {
        Items = items;
        Success = success;
        Message = message;
    }
}
