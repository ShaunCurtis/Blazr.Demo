/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record ListProviderResult<TRecord>
{
    public IEnumerable<TRecord> Items { get; init; } = Enumerable.Empty<TRecord>();

    public int TotalItemCount { get; init; } = 0;

    public bool Success { get; init; } = false;

    public string Message { get; init; } = String.Empty;

    public ListProviderResult() { }

    public static ListProviderResult<TRecord> Failure(string message)
        => new ListProviderResult<TRecord> { Message = message };

    public static ListProviderResult<TRecord> Successful(IEnumerable<TRecord> items, int totalItemCount, string? message = null)
        => new ListProviderResult<TRecord> { Items = items, TotalItemCount = totalItemCount, Success = true, Message = message ?? "The query completed successfully" };
}
