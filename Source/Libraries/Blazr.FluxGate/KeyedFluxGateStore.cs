/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.FluxGate;

public class KeyedFluxGateStore<TState, TKey>
    where TState : new()
    where TKey : notnull
{
    private readonly FluxGateDispatcher<TState> _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    private Dictionary<TKey, FluxGateStore<TState>> _items = new();

    public KeyedFluxGateStore(IServiceProvider serviceProvider, FluxGateDispatcher<TState> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        _serviceProvider = serviceProvider;
    }

    public FluxGateStore<TState>? GetStore(TKey key)
    {
        if (_items.TryGetValue(key, out FluxGateStore<TState>? store))
            return store;

        return default;
    }

    public FluxGateStore<TState> GetOrCreateStore(TKey key)
    {
        FluxGateStore<TState>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        store = (FluxGateStore<TState>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<TState>));

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TState).Name}.");

        _items.Add(key, store);

        return store;
    }

    public FluxGateStore<TState> GetOrCreateStore(TKey key, TState initialState)
    {
        FluxGateStore<TState>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        ArgumentNullException.ThrowIfNull(initialState);

        store = (FluxGateStore<TState>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<TState>), initialState);

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TState).Name}.");

        _items.Add(key, store);

        return store;
    }

    public bool RemoveStore(TKey key)
    {
        if (_items.ContainsKey(key))
        {
            _items.Remove(key);
            return true;
        }
        return false;
    }

    public void Dispatch(TKey key, IFluxGateAction action)
    {
        var store = this.GetOrCreateStore(key);
        store.Dispatch(action);
    }
}
