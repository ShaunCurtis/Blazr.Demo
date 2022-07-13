/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class UiStateService
{
    private Dictionary<Guid, object> _stateItems = new Dictionary<Guid, object>();

    public void AddStateData(Guid Id, object value) 
    {
        if (Id == Guid.Empty)
            return;

        if (_stateItems.ContainsKey(Id))
            _stateItems[Id] = value;
        else
            _stateItems.Add(Id, value);
    }

    public void ClearStateDataData(Guid Id)
    {
        if (Id == Guid.Empty)
            return;

        if (_stateItems.ContainsKey(Id))
            _stateItems.Remove(Id);
    }

    public bool TryGetStateData<T>(Guid Id, out T? value)
    {
        value = default;

        if (Id == Guid.Empty)
            return false;

        var isdata = _stateItems.ContainsKey(Id);

        var val = isdata
            ? _stateItems[Id]
            : default;

        if (val is T)
        {
            value = (T)val;
            return true;
        }

        return false;
    }
}

