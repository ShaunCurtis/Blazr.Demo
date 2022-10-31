/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core.Validation;
using System;

namespace Blazr.Core;

public abstract class RecordEditContextBase<TRecord> : IRecordEditContext
    where TRecord : class, new()
{
    protected TRecord BaseRecord = new();

    public bool ValidateOnFieldChanged { get; set; } = false;

    public virtual TRecord CurrentRecord => new();

    public readonly ValidationMessageCollection ValidationMessages = new();

    public virtual bool IsDirty => !BaseRecord.Equals(CurrentRecord);

    public bool HasMessages(string? fieldName = null)
        => this.ValidationMessages.HasMessages(fieldName);

    public IEnumerable<string> GetMessages(string? field = null)
        => this.ValidationMessages.GetMessages(field);

    public event EventHandler<string?>? FieldChanged;
    public event EventHandler<ValidationStateEventArgs>? ValidationStateUpdated;

    public RecordEditContextBase(TRecord record)
        => this.Load(record);

    public abstract void Load(TRecord record);

    public abstract void Reset();

    protected void SetIfChanged<TType>(ref TType currentValue, TType value, string fieldName)
    {
        if (value is null || currentValue is null || !value.Equals(currentValue))
        {
            currentValue = value;
            NotifyFieldChanged(fieldName);
        }
    }

    public void NotifyFieldChanged(string fieldName)
    {
        if (this.ValidateOnFieldChanged)
            this.Validate(fieldName);

        FieldChanged?.Invoke(null, fieldName);
    }

    public void NotifyValidationStateUpdated(bool state, string? field)
        => ValidationStateUpdated?.Invoke(null, ValidationStateEventArgs.Create(state, field));

    public virtual ValidationResult Validate(string? fieldname = null)
        => new ValidationResult { IsValid = ValidationMessages.HasMessages(), ValidationMessages = ValidationMessages, ValidationNotRun = false };
}
