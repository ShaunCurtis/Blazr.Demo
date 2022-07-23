/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

//TODO - is this code redundant?
namespace Blazr.UI;

public abstract class BaseStateClass<TStateRecord>
    : IStateClass<TStateRecord>
    where TStateRecord : BaseStateRecord, new()
{
    protected TStateRecord SavedState = new();

    private TStateRecord _state = new();
    public TStateRecord State
    {
        get => _state;
        protected set
        {
            _state = value;
            NotifyStateChange();
        }
    }

    public event EventHandler? StateHasChanged;

    public void Set(TStateRecord record)
    {
        this.SavedState = record with { };
        this.State = record with { };
    }

    private void NotifyStateChange()
    {
        if (this.SavedState != _state)
        {
            this.StateHasChanged?.Invoke(this, EventArgs.Empty);
            this.SavedState = _state;
        }
    }
}
