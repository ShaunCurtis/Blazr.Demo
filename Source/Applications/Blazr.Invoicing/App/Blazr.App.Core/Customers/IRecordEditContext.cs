/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public interface IRecordEditContext<TRecord>
    where TRecord : class
{
    public TRecord AsRecord { get; }

    public bool IsDirty { get; }

    public void Load(TRecord record);
}
