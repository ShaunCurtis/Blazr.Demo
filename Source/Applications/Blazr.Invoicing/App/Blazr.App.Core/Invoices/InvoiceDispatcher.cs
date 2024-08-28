/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.FluxGate;

namespace Blazr.App.Core;

public readonly record struct UpdateInvoiceAction(DmoInvoice Item) : IFluxGateAction;
public readonly record struct DeleteInvoiceAction() : IFluxGateAction;
public readonly record struct SetInvoiceAsPersistedAction() : IFluxGateAction;
public readonly record struct UpdateInvoicePriceAction(decimal TotalAmount) : IFluxGateAction;

public class InvoiceDispatcher : FluxGateDispatcher<DmoInvoice>
{
    public override FluxGateResult<DmoInvoice> Dispatch(FluxGateStore<DmoInvoice> store, IFluxGateAction action)
    {
        return action switch
        {
            UpdateInvoiceAction a1 => Mutate(store, a1),
            UpdateInvoicePriceAction a2 => Mutate(store, a2),
            DeleteInvoiceAction a3 => Mutate(store, a3),
            SetInvoiceAsPersistedAction a1 => Mutate(store, a1),
            _ => new FluxGateResult<DmoInvoice>(false, store.Item, store.State)
        };
    }

    private static FluxGateResult<DmoInvoice> Mutate(FluxGateStore<DmoInvoice> store, UpdateInvoicePriceAction action)
    {
        var updatedItem = store.Item with { TotalAmount = action.TotalAmount };
        var state = store.State.Modified();

        return new FluxGateResult<DmoInvoice>(true, updatedItem, state);
    }

    private static FluxGateResult<DmoInvoice> Mutate(FluxGateStore<DmoInvoice> store, UpdateInvoiceAction action)
    {
        if (action.Item.Id != store.Item.Id)
            return new FluxGateResult<DmoInvoice>(false, store.Item, store.State, "Invoice Id's don't match.");

        var state = store.State.Modified();

        return new FluxGateResult<DmoInvoice>(true, action.Item, state);
    }

    private static FluxGateResult<DmoInvoice> Mutate(FluxGateStore<DmoInvoice> store, DeleteInvoiceAction action)
    {
        var state = store.State.Deleted();

        return new FluxGateResult<DmoInvoice>(true, store.Item, state);
    }

    private static FluxGateResult<DmoInvoice> Mutate(FluxGateStore<DmoInvoice> store, SetInvoiceAsPersistedAction action)
    {
        var state = FluxGateState.AsExisting();

        return new FluxGateResult<DmoInvoice>(true, store.Item, state);
    }
}

