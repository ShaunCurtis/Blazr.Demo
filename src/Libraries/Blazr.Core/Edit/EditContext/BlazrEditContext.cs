/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public abstract class BlazrEditContext<TRecord> : IBlazrRecordEditContext<TRecord>, IBlazrEditContext
    where TRecord : class, IStateEntity, IEntity, new()
{
    private bool _isMarkedForDeletion;
    public EntityUid Uid { get; protected set; }

    public EntityState EntityState => this.BaseRecord.EntityState with { 
        IsMutated = this.IsDirty,
        MarkedForDeletion = _isMarkedForDeletion
    }; 

    public TRecord BaseRecord { get; protected set; } = default!;

    public bool IsDirty => this.BaseRecord != this.MapToRecord();

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
        => _isMarkedForDeletion = true;

    void IBlazrEditContext.SetAsSaved()
    {
        this.BaseRecord = this.AsRecord;
    }

    protected abstract void MapToContext(TRecord record);

    protected abstract TRecord MapToRecord();
}
