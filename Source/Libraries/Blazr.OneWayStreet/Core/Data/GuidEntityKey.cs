/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core.Data;

public record GuidEnitityKey 
{
    public Guid Value { get; init; }

    public object KeyValue => this.Value;

    public GuidEnitityKey()
    {
        this.Value = Guid.Empty;
    }

    public GuidEnitityKey(Guid value)
    {
        this.Value = value;
    }

    public static GuidEnitityKey NewEntity()
        => new(Guid.Empty);
}
