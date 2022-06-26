/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IEditRecord<TRecord> 
    where TRecord : class, new()
{
    public Guid Id { get; set; }

    public TRecord Record { get; }

    public TRecord AsNewRecord { get; }

    public bool IsDirty { get; }

    public bool IsNew { get; }

    public void Load(TRecord record);

    public void Reset();
}
