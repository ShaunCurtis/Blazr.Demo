/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate;

public abstract class FluxGateDispatcher<TState>
{
    public abstract TState Dispatch(TState state, IFluxGateAction action);
}
