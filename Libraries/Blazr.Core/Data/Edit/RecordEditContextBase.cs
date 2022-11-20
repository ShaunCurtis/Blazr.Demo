/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core.Data.Validation;
using Blazr.Core.Validation;

namespace Blazr.Core.Edit;

public abstract class RecordEditContextBase<TRecord> : IEditRecord<TRecord>, IRecordEditContext<TRecord>, IEditContext, IValidation
    where TRecord : class, new()
{
    protected TRecord BaseRecord = new();

    public Guid InstanceId { get; } = Guid.NewGuid();

    public virtual Guid Uid { get; set; }

    public bool ValidateOnFieldChanged { get; set; } = false;

    public virtual TRecord Record => new();

    public readonly ValidationMessageCollection ValidationMessages = new();
    public readonly PropertyStateCollection PropertyStates = new();

    public virtual bool IsDirty => !BaseRecord.Equals(this.Record);

    public bool IsNew => this.Uid == Guid.Empty;

    public TRecord CleanRecord => this.BaseRecord;

    public abstract TRecord AsNewRecord();

    public bool HasMessages(string? fieldName = null)
        => this.ValidationMessages.HasMessages(fieldName);

    public IEnumerable<string> GetMessages(string? field = null)
        => this.ValidationMessages.GetMessages(field);

    public event EventHandler<string?>? FieldChanged;
    public event EventHandler<ValidationStateEventArgs>? ValidationStateUpdated;
    public event EventHandler<bool>? EditStateUpdated;

    public RecordEditContextBase() { }

    public RecordEditContextBase(TRecord record)
        => this.Load(record);

    public abstract void Load(TRecord record, bool notify = true);

    public void Reset()
        => this.Load(this.BaseRecord);

    protected bool UpdateifChangedAndNotify<TType>(ref TType currentValue, TType value, TType originalValue, string fieldName)
    {
        var hasChanged = !value?.Equals(currentValue) ?? currentValue is not null;
        var hasChangedFromOriginal = !value?.Equals(originalValue) ?? originalValue is not null;
        if (hasChanged)
        {
            currentValue = value;
            NotifyFieldChanged(fieldName);
        }

        var field = FieldReference.Create(this.InstanceId, fieldName);
        this.PropertyStates.ClearState(field);
        if (hasChangedFromOriginal)
            this.PropertyStates.Add(field);

        return hasChanged;
    }

    public void NotifyFieldChanged(string? fieldName)
    {
        FieldChanged?.Invoke(null, fieldName);
        EditStateUpdated?.Invoke(null, IsDirty);
    }

    public void NotifyValidationStateUpdated(bool state, string? field)
        => ValidationStateUpdated?.Invoke(null, ValidationStateEventArgs.Create(state, field));

    public virtual ValidationResult Validate(string? fieldname = null)
        => new ValidationResult { IsValid = ValidationMessages.HasMessages(), ValidationMessages = ValidationMessages, ValidationNotRun = false };
}
