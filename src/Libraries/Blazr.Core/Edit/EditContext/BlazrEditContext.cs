/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public abstract class BlazrEditContext<TRecord> : IBlazrRecordEditContext<TRecord>, IBlazrEditContext
    where TRecord : class, IStateEntity, IEntity, new()
{
    public Guid Uid { get; protected set; } = Guid.NewGuid();

    public int StateCode => internalStateCode;

    public TRecord BaseRecord { get; protected set; } = default!;

    public bool IsDirty => this.BaseRecord != this.MapToRecord();

    protected int internalStateCode = AppStateCodes.Record;

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
        internalStateCode = AppStateCodes.Delete;
    }

    void IBlazrEditContext.SetAsSaved()
    {
        this.BaseRecord = this.AsRecord;
        internalStateCode = AppStateCodes.Record;
    }

    protected abstract void MapToContext(TRecord record);

    protected abstract TRecord MapToRecord();
}
