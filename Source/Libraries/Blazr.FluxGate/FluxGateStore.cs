/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.FluxGate;

public class FluxGateStore<TState>
    where TState : new()
{
    private readonly FluxGateDispatcher<TState> _dispatcher;

    public TState Item { get; private set; }
    public event EventHandler<FluxGateEventArgs>? StateChanged;

    public FluxGateStore(FluxGateDispatcher<TState> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        this.Item = new();
    }

    public FluxGateStore(FluxGateDispatcher<TState> fluxStateDispatcher, TState state)
    {
        _dispatcher = fluxStateDispatcher;
        this.Item = state;
    }

    public void Dispatch(IFluxGateAction action)
    {
        this.Item = _dispatcher.Dispatch(this.Item, action);

        this.StateChanged?.Invoke(action, new FluxGateEventArgs() { State = this.Item });
    }
}
