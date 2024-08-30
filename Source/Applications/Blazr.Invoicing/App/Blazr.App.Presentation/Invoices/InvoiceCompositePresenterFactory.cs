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
    private readonly INewRecordProvider<DmoInvoice> _newInvoiceProvider;
    private readonly INewRecordProvider<DmoInvoiceItem> _newInvoiceItemProvider;
    private readonly InvoiceCompositeFactory _factory;

    public InvoiceCompositePresenterFactory(InvoiceCompositeFactory invoiceCompositeFactory, IToastService toastService,
        IDataBroker dataBroker, INewRecordProvider<DmoInvoice> newInvoiceProvider, INewRecordProvider<DmoInvoiceItem> newInvoiceItemProvider)
    {
        _factory = invoiceCompositeFactory;
        _toastService = toastService;
        _dataBroker = dataBroker;
        _newInvoiceProvider = newInvoiceProvider;
        _newInvoiceItemProvider = newInvoiceItemProvider;
    }

    public async ValueTask<InvoiceCompositePresenter> CreateInstanceAsync(InvoiceId id)
    {
        var presenter = new InvoiceCompositePresenter(_factory, _toastService, _dataBroker, _newInvoiceProvider);
        await presenter.LoadAsync(id);

        // Set the current context on the scoped NewInvoiceItemProvider
        if (_newInvoiceItemProvider is NewInvoiceItemProvider provider)
            provider.SetInvoiceContext(id);

        return presenter;
    }
}
