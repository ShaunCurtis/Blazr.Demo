/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public readonly record struct ItemQueryAPIRequest<TKey>
{
    public TKey KeyValue { get; init; }

    public ItemQueryAPIRequest()
    {
        this.KeyValue = default!;
    }

    public ItemQueryAPIRequest(TKey keyValue)
    {
        this.KeyValue = keyValue;
    }

    public static ItemQueryAPIRequest<TKey> FromRequest(ItemQueryRequest<TKey> query)
        => new()
        {
            KeyValue = query.Key
        };

    public ItemQueryRequest<TKey> ToRequest(CancellationToken? cancellation = null)
        => new()
        {
            Key = this.KeyValue ?? default!,
            Cancellation = cancellation ?? CancellationToken.None
        };
}
