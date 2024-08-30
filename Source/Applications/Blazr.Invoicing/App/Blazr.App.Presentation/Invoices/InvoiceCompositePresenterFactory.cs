/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceCompositePresenterFactory
{
    private readonly IDataBroker _dataBroker;
    private readonly IToastService _toastService;
    private readonly INewRecordProvider<DmoInvoice> _recordProvider;
    private readonly InvoiceCompositeFactory _factory;

    internal InvoiceCompositePresenterFactory(InvoiceCompositeFactory invoiceCompositeFactory, IToastService toastService, IDataBroker dataBroker, INewRecordProvider<DmoInvoice> recordProvider)
    {
        _toastService = toastService;
        _dataBroker = dataBroker;
        _recordProvider = recordProvider;
        _factory = invoiceCompositeFactory;
    }

    public async ValueTask<InvoiceCompositePresenter> CreateInstanceAsync(InvoiceId id)
    {
        var presenter = new InvoiceCompositePresenter(_factory, _toastService, _dataBroker,_recordProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }
}
