/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate;

public abstract class FluxGateDispatcher<TFluxGateItem>
    where TFluxGateItem : new()
{
    public abstract FluxGateResult<TFluxGateItem> Dispatch(FluxGateStore<TFluxGateItem> store, IFluxGateAction action);
}
