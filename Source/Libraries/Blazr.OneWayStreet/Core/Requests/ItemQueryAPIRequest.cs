/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public readonly record struct ItemQueryAPIRequest<IKey>
{
    public IKey KeyValue { get; init; }

    public ItemQueryAPIRequest()
    {
        this.KeyValue = default!;
    }

    public ItemQueryAPIRequest(IKey keyValue)
    {
        this.KeyValue = keyValue;
    }

    public static ItemQueryAPIRequest<IKey> FromRequest(ItemQueryRequest<IKey> request)
        => new(request.KeyValue);

    public ItemQueryRequest<IKey> ToRequest(CancellationToken? cancellation = null)
        => new()
        {
            KeyValue = this.KeyValue,
            Cancellation = cancellation ?? CancellationToken.None
        };

}
