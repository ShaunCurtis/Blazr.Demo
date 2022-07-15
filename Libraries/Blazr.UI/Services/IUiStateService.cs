/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public interface IUiStateService
{
    public void AddStateData(Guid Id, object value);

    public void ClearStateDataData(Guid Id);

    public bool TryGetStateData<T>(Guid Id, out T? value);
}

