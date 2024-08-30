/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.FluxGate;

namespace Blazr.App.Core;

public readonly record struct AddInvoiceItemAction(object Sender, DmoInvoiceItem Item) : IFluxGateAction;
public readonly record struct UpdateInvoiceItemAction(object Sender, DmoInvoiceItem Item) : IFluxGateAction;
public readonly record struct DeleteInvoiceItemAction(object Sender) : IFluxGateAction;
public readonly record struct SetInvoiceItemAsPersistedAction(object Sender) : IFluxGateAction;

public class InvoiceItemDispatcher : FluxGateDispatcher<DmoInvoiceItem>
{
    public override FluxGateResult<DmoInvoiceItem> Dispatch(FluxGateStore<DmoInvoiceItem> store, IFluxGateAction action)
    {
        return action switch
        {
            UpdateInvoiceItemAction a => Mutate(store, a),
            DeleteInvoiceItemAction a => Mutate(store, a),
            SetInvoiceItemAsPersistedAction a => Mutate(store, a),
            _ => new FluxGateResult<DmoInvoiceItem>(false, store.Item, store.State)
        };
    }

    private static FluxGateResult<DmoInvoiceItem> Mutate(FluxGateStore<DmoInvoiceItem> store, UpdateInvoiceItemAction action)
    {
        if (action.Item.InvoiceItemId != store.Item.InvoiceItemId)
            return new FluxGateResult<DmoInvoiceItem>(false, store.Item, store.State, "Invoice Item Id's don't match.");

        var state = store.State.Modified();

        return new FluxGateResult<DmoInvoiceItem>(true, action.Item, state);
    }

    private static FluxGateResult<DmoInvoiceItem> Mutate(FluxGateStore<DmoInvoiceItem> store, DeleteInvoiceItemAction action)
    {
        var state = store.State.Deleted();

        return new FluxGateResult<DmoInvoiceItem>(true, store.Item, state);
    }

    private static FluxGateResult<DmoInvoiceItem> Mutate(FluxGateStore<DmoInvoiceItem> store, SetInvoiceItemAsPersistedAction action)
    {
        var state = FluxGateState.AsExisting();

        return new FluxGateResult<DmoInvoiceItem>(true, store.Item, state);
    }
}

