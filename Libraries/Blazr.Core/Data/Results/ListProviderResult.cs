/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record ListProviderResult<TRecord>
{
    public IEnumerable<TRecord> Items { get; init; } = Enumerable.Empty<TRecord>();

    public int TotalItemCount { get; init; }

    public bool Success { get; init; }

    public string? Message { get; init; }

    public ListProviderResult() { }

    public ListProviderResult(IEnumerable<TRecord> items, int totalItemCount, bool success = true, string? message = null)
    {
        Items = items;
        TotalItemCount = totalItemCount;
        Success = success;
        Message = message;
    }
}
