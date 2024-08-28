/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate;

public struct FluxGateState
{
    private bool _isNew;
    private bool _isModified;
    private bool _isDeleted;

    public readonly bool IsNew => _isNew;
    public bool IsDeleted => _isDeleted;
    public bool IsModified => _isModified;

    public FluxGateState() { }

    public FluxGateState Modified(bool value = true)
        => new FluxGateState() { _isNew = this.IsNew, _isDeleted = this.IsDeleted, _isModified = value };

    public FluxGateState Deleted(bool value = true)
        => new FluxGateState() { _isNew = this.IsNew, _isDeleted = value, _isModified = this.IsModified };

    public static FluxGateState AsNew() => new FluxGateState() { _isNew = true };
    public static FluxGateState AsExisting() => new FluxGateState();
}