/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Edit;

/// <summary>
/// The PropertyStateCollection is a IEnumerable custom collection used to manage
/// state information.  Any field (represented by a `FieldReference` object) in the collection, is dirty.
/// </summary>
public class PropertyStateCollection : IEnumerable<FieldReference>
{
    private readonly List<FieldReference> _states = new List<FieldReference>();

    public void Add(FieldReference state)
        => _states.Add(state);

    public void Add(Guid objectUid, string field)
        => _states.Add(FieldReference.Create(objectUid, field));

    public void ClearState(FieldReference field)
    {
        var toDelete = _states.Where(item => item.Equals(field)).ToList();
        if (toDelete is not null)
            foreach (var state in toDelete)
                _states.Remove(state);
    }

    public void ClearAllStates()
        => _states.Clear();

    public bool GetState(FieldReference field)
        => _states.Any(item => item.Equals(field));

    public bool HasState(Guid? objectUid = null)
        => objectUid is null
            ? _states.Any()
            : _states.Any(item => item.ObjectUid.Equals(objectUid));

    public IEnumerator<FieldReference> GetEnumerator()
        => _states.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
