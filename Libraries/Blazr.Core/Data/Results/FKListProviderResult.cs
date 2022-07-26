/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record FKListProviderResult
{
    public IEnumerable<IFkListItem> Items { get; init; } = Enumerable.Empty<IFkListItem>();

    public bool Success { get; init; }

    public string? Message { get; init; }

    public FKListProviderResult() { }

    public FKListProviderResult(IEnumerable<IFkListItem> items, bool success = true, string? message = null)
    {
        Items = items;
        Success = success;
        Message = message;
    }
}
