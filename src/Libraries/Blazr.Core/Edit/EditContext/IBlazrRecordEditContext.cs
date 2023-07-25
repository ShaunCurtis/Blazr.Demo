/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IBlazrRecordEditContext<TRecord>
    where TRecord : class, IStateEntity, IEntity, new()
{
    public EntityUid Uid { get; }
    public int StateCode { get;}
    public TRecord BaseRecord { get; }
    public bool IsDirty { get; }
    public TRecord AsRecord { get; }
    public void Load(TRecord record);
    public void SetAsDeleted();
}
