/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record FKListProviderResult<TFkListItem>
    where TFkListItem :IFkListItem
{
    public IEnumerable<TFkListItem> Items { get; private init; } = Enumerable.Empty<TFkListItem>();

    public bool Success { get; private init; }

    public string? Message { get; private init; }

    public FKListProviderResult() { }

    public static FKListProviderResult<TFkListItem> Failure(string message)
        => new FKListProviderResult<TFkListItem> { Message = message };

    public static FKListProviderResult<TFkListItem> Successful(IEnumerable<TFkListItem> items, string? message = null)
        => new FKListProviderResult<TFkListItem> { Items = items, Success = true, Message = message ?? "The query completed successfully" };
}
