/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct ListProviderResult<TItem>
{
    public IEnumerable<TItem> Items { get; }

    public int TotalItemCount { get; }

    public bool Success { get; }

    public string? Message { get; }

    public ItemsProviderResult<TItem> ItemsProviderResult => new ItemsProviderResult<TItem>(this.Items, this.TotalItemCount);

    public ListProviderResult(IEnumerable<TItem> items, int totalItemCount, bool success = true, string? message = null)
    {
        Items = items;
        TotalItemCount = totalItemCount;
        Success = success;
        Message = message;
    }
}
