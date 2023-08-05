/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet;

public class OwsEntityStore<TEntity>
    where TEntity : class, IOwsEntity
{
    // Contains all the current entitiy instancies
    private readonly List<OwsEntity<TEntity>> _Entities = new();
    private int _entityTimeout = 30;

    public event EventHandler<OwsStateChangeEventArgs>? StateHasChanged;

    /// <summary>
    /// Adds the initial entity instance to the store
    /// and returns a handle to the instance
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public void AddAState(TEntity entity)
    {
        this.ThrowIfExists(entity);

        _Entities.Add(new OwsEntity<TEntity>(entity));
    }

    /// <summary>
    /// Removes an instance from the Store
    /// </summary>
    /// <param name="uid"></param>
    public void RemoveAnEntity(EntityUid uid)
    {
        var record = _Entities.FirstOrDefault(item => item.EntityUid == uid);

        if (record != null)
            _Entities.Remove(record);
    }

    /// <summary>
    /// Gets a reference to the current state
    /// Note that this is only valid at the time of the request.
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public TEntity? GetEntity(EntityUid uid)
        => _Entities.FirstOrDefault(item => item.EntityUid == uid)?.State ?? null;

    /// <summary>
    /// The dispatcher for the entity
    /// </summary>
    /// <param name="uid">The Uid of the entity</param>
    /// <param name="mutation">The mutation delegate to apply to the entity</param>
    /// <returns>The mutated State</returns>
    /// <exception cref="StateDoesNotExistsException">Raised if the entity does not exist in the store</exception>
    public async Task<TEntity> DispatchAsync(EntityUid uid, OwsEntityMutationDelegate<TEntity> mutation)
    {
        var entity = _Entities.FirstOrDefault(item => item.EntityUid == uid);
        if (entity is null)
            throw new StateDoesNotExistsException($"A state object does not exist for identity {uid}");

        var task = entity.DispatchAsync(mutation);

        this.DoHousekeeping();

        var newEntity = await task;

        this.StateHasChanged?.Invoke(this, new OwsStateChangeEventArgs(uid, newEntity));

        return newEntity;
    }

    /// <summary>
    /// Methods to set the timeout period in minutes for an entity state
    /// Each transaction on an entity sets it's timestamp
    /// If an entity timeout expires the entitt object and all it's resources 
    /// will be deallocated for the GC to remove 
    /// </summary>
    /// <param name="minutes"></param>
    public void SetEntityTimeOut(int minutes)
        => _entityTimeout = minutes;

    private void DoHousekeeping()
    {
        // Get any entities that haven't been accessed in the timneour period and remove them
        var deletes = _Entities.Where(item => item.LastActivity > DateTime.Now.AddMinutes(_entityTimeout));
        foreach (var item in deletes)
            _Entities.Remove(item);
    }

    private void ThrowIfExists(TEntity entity)
    {
        if (_Entities.Any(item => item.EntityUid == entity.EntityUid))
            throw new EntityAlreadyExistsException($"An enitiy object already exists for identity {entity.EntityUid}");
    }

    private void ThrowIfDoesNotExist(TEntity entity)
    {
        if (!_Entities.Any(item => item.EntityUid == entity.EntityUid))
            throw new StateDoesNotExistsException($"An entity object does not exist for identity {entity.EntityUid}");
    }
}
