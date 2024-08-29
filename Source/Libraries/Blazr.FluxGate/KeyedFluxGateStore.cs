/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.FluxGate;

public class KeyedFluxGateStore<TFluxGateItem, TKey>
    where TFluxGateItem : new()
    where TKey : notnull
{
    private readonly FluxGateDispatcher<TFluxGateItem> _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    private Dictionary<TKey, FluxGateStore<TFluxGateItem>> _items = new();

    public IEnumerable<TFluxGateItem> Items => _items.Select(item => item.Value.Item).AsEnumerable();
    public IEnumerable<FluxGateStore<TFluxGateItem>> Stores => _items.Select(item => item.Value).AsEnumerable();

    public KeyedFluxGateStore(IServiceProvider serviceProvider, FluxGateDispatcher<TFluxGateItem> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        _serviceProvider = serviceProvider;
    }

    public FluxGateStore<TFluxGateItem>? GetStore(TKey key)
    {
        if (_items.TryGetValue(key, out FluxGateStore<TFluxGateItem>? store))
            return store;

        return default;
    }

    public FluxGateStore<TFluxGateItem> GetOrCreateStore(TKey key)
    {
        FluxGateStore<TFluxGateItem>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        store = (FluxGateStore<TFluxGateItem>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<TFluxGateItem>));

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TFluxGateItem).Name}.");

        _items.Add(key, store);

        return store;
    }

    public FluxGateStore<TFluxGateItem> GetOrCreateStore(TKey key, TFluxGateItem initialState, bool isNew = false)
    {
        FluxGateStore<TFluxGateItem>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        ArgumentNullException.ThrowIfNull(initialState);

        store = (FluxGateStore<TFluxGateItem>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<TFluxGateItem>), new object[] { initialState, isNew });

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TFluxGateItem).Name}.");

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

    public FluxGateResult<TFluxGateItem> Dispatch(TKey key, IFluxGateAction action)
    {
        var store = this.GetOrCreateStore(key);
        return store.Dispatch(action);
    }
}
