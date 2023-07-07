/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public abstract class BlazrEditContext<TRecord> : IBlazrRecordEditContext<TRecord>, IBlazrEditContext
    where TRecord : class, IStateEntity, IGuidIdentity, new()
{
    public Guid Uid { get; protected set; } = Guid.NewGuid();

    public int StateCode => internalStateCode;

    public TRecord BaseRecord { get; protected set; } = default!;

    public bool IsDirty => this.BaseRecord != this.MapToRecord();

    protected int internalStateCode = StateCodes.Record;

    public virtual TRecord AsRecord
        => this.MapToRecord();

    public BlazrEditContext()
    {
        this.BaseRecord = new();
        this.MapToContext(this.BaseRecord);
    }

    public void Load(TRecord record)
    {
        this.BaseRecord = record;
        this.MapToContext(this.BaseRecord);
    }

    void IBlazrEditContext.Reset()
        => this.MapToContext(this.BaseRecord);

    public void SetAsDeleted()
    {
        internalStateCode = StateCodes.Delete;
    }

    void IBlazrEditContext.SetAsSaved()
    {
        this.BaseRecord = this.AsRecord;
        internalStateCode = StateCodes.Record;
    }

    protected abstract void MapToContext(TRecord record);

    protected abstract TRecord MapToRecord();
}
