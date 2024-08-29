/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.FluxGate;

public class FluxGateStore<TFluxGateItem>
    where TFluxGateItem : new()
{
    private readonly FluxGateDispatcher<TFluxGateItem> _dispatcher;

    public TFluxGateItem Item { get; private set; }
    public FluxGateState State { get; private set; } = FluxGateState.AsNew();
    public event EventHandler<FluxGateEventArgs>? StateChanged;

    public FluxGateStore(FluxGateDispatcher<TFluxGateItem> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        this.Item = new();
    }

    public FluxGateStore(FluxGateDispatcher<TFluxGateItem> fluxStateDispatcher, TFluxGateItem state)
    {
        _dispatcher = fluxStateDispatcher;
        this.Item = state;
        this.State = FluxGateState.AsExisting();
    }

    public FluxGateStore(FluxGateDispatcher<TFluxGateItem> fluxStateDispatcher, TFluxGateItem state, bool isNew)
    {
        _dispatcher = fluxStateDispatcher;
        this.Item = state;
        this.State = isNew ? FluxGateState.AsNew() : FluxGateState.AsExisting();
    }

    public FluxGateResult<TFluxGateItem> Dispatch(IFluxGateAction action)
    {
        var result = _dispatcher.Dispatch(this, action);
        if (result.Success)
        {
            this.Item = result.Item;
            this.State = result.State;

            this.StateChanged?.Invoke(action.Sender, new FluxGateEventArgs() { State = this.Item });
        }
        return result;
    }
}
