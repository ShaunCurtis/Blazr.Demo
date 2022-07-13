/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public interface IStateClass<TStateRecord>
where TStateRecord : BaseStateRecord, new()
{
    public TStateRecord State { get; }

    public event EventHandler? StateHasChanged;

    public void Set(TStateRecord record);
}
