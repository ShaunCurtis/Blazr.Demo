/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public readonly record struct ItemQueryRequest
{
    public object KeyValue  { get; init; }
    public CancellationToken Cancellation { get; init; }

    public ItemQueryRequest(object? keyValue, CancellationToken? cancellation = null)
    {
        this.KeyValue = keyValue ?? Guid.Empty;
        this.Cancellation = cancellation ?? new(); ;
    }
    public static ItemQueryRequest Create(object keyValue, CancellationToken? cancellation = null)
        => new ItemQueryRequest(keyValue, cancellation ?? new());
}

public readonly record struct ItemQueryRequest<TKey>
{
    public TKey KeyValue { get; init; }
    public CancellationToken Cancellation { get; init; }

    public ItemQueryRequest(TKey keyValue, CancellationToken? cancellation = null)
    {
        this.KeyValue = keyValue;
        this.Cancellation = cancellation ?? new(); ;
    }
    public static ItemQueryRequest<TKey> Create(TKey keyValue, CancellationToken? cancellation = null)
        => new ItemQueryRequest<TKey>(keyValue, cancellation ?? new());
}
