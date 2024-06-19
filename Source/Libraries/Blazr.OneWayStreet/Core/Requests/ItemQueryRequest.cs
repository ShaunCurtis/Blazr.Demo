/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public readonly record struct ItemQueryRequest
{
    public object Key  { get; init; }
    public CancellationToken Cancellation { get; init; }

    public ItemQueryRequest(object? key, CancellationToken? cancellation = null)
    {
        this.Key = key ?? Guid.Empty;
        this.Cancellation = cancellation ?? new(); ;
    }
    public static ItemQueryRequest Create(object keyValue, CancellationToken? cancellation = null)
        => new ItemQueryRequest(keyValue, cancellation ?? new());
}

public readonly record struct ItemQueryRequest<TKey>
    where TKey : IEntityKey
{
    public TKey Key { get; init; }
    public CancellationToken Cancellation { get; init; }

    public ItemQueryRequest(TKey key, CancellationToken? cancellation = null)
    {
        this.Key = key;
        this.Cancellation = cancellation ?? new(); ;
    }
    public static ItemQueryRequest<TKey> Create(TKey keyValue, CancellationToken? cancellation = null)
        => new ItemQueryRequest<TKey>(keyValue, cancellation ?? new());
}
