/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IReadService<TRecord>
    where TRecord : class, new()
{
    public TRecord? Record { get; }

    public string? Message { get; }

    public bool HasRecord => this.Record is not null;

    public ValueTask GetRecordAsync(Guid Id);
}

