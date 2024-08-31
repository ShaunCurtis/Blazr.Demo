/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceItemEditPresenterFactory
{
    private readonly IToastService _toastService;

    public InvoiceItemEditPresenterFactory(IToastService toastService)
    {
        _toastService = toastService;
    }

    public InvoiceItemEditPresenter CreateInstance(InvoiceComposite composite, InvoiceItemId id)
    {
        var presenter = new InvoiceItemEditPresenter(composite, _toastService, id);

        return presenter;
    }

}
