﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceCompositePresenter
{
    private readonly IDataBroker _dataBroker;
    private readonly IToastService _toastService;
    private readonly INewRecordProvider<DmoInvoice> _recordProvider;
    private readonly InvoiceCompositeFactory _factory;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();

    public InvoiceComposite Composite { get; private set; }

    public IQueryable<DmoInvoiceItem> InvoiceItems => this.Composite.InvoiceItems.AsQueryable();

    internal InvoiceCompositePresenter(InvoiceCompositeFactory invoiceCompositeFactory, IToastService toastService, IDataBroker dataBroker, INewRecordProvider<DmoInvoice> recordProvider)
    {
        _toastService = toastService;
        _dataBroker = dataBroker;
        _recordProvider = recordProvider;
        _factory = invoiceCompositeFactory;

        // Build a new context
        this.Composite = _factory.GetInstance(_recordProvider.NewRecord(), Enumerable.Empty<DmoInvoiceItem>(), true);
    }

    internal async Task LoadAsync(InvoiceId id)
    {
        this.LastDataResult = DataResult.Success();

        // if we have an empty guid them we go with the new context created in the constructor
        if (id.Value != Guid.Empty)
        {
            var request = ItemQueryRequest<InvoiceId>.Create(id);
            var result = await _dataBroker.ExecuteQueryAsync<InvoiceComposite, InvoiceId>(request);
            LastDataResult = result;
            if (this.LastDataResult.Successful)
            {
                this.Composite = result.Item!;
            }
            return;
        }
    }
}
