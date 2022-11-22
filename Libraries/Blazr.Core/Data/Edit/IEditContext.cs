/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core.Validation;

namespace Blazr.Core.Edit;

/// <summary>
/// The purpose of IEditContext is to provide an interface on an Edit Context
/// that generic components can use without need to know of the underlying record types
/// defined in implementation classes.
/// 
/// Examples are:
/// 
///   -  validation components that display validation infornmation
///   -  input controls that change appearance when they have been edited or are valid/invalid
///   
/// </summary>
public interface IEditContext
{
    public event EventHandler<ValidationStateEventArgs>? ValidationStateUpdated;
    public event EventHandler<bool>? EditStateUpdated;
    public event EventHandler<string?>? FieldChanged;
    public Guid Uid { get; set; }

    public bool IsDirty { get; }

    public bool IsNew { get; }

    public bool HasMessages(FieldReference? field = null);
    public bool IsChanged(FieldReference field);
    public IEnumerable<string> GetMessages(FieldReference? field = null);
    public ValidationResult Validate(FieldReference? field = null);
}