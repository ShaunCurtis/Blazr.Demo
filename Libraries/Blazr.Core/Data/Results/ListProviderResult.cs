/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct ListProviderResult<TRecord>
{
    public IEnumerable<TRecord> Items { get; }

    public int TotalItemCount { get; }

    public bool Success { get; }

    public string? Message { get; }

    public ItemsProviderResult<TRecord> ItemsProviderResult => new ItemsProviderResult<TRecord>(this.Items, this.TotalItemCount);

    public ListProviderResult(IEnumerable<TRecord> items, int totalItemCount, bool success = true, string? message = null)
    {
        Items = items;
        TotalItemCount = totalItemCount;
        Success = success;
        Message = message;
    }
}
