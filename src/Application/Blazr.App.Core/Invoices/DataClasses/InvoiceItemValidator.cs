/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class InvoiceItemValidator : AbstractValidator<InvoiceItemEditContext>
{
    public InvoiceItemValidator()
    {
        this.RuleFor(p => p.ProductUid)
            .NotEmpty()
            .WithState(p => p);

        this.RuleFor(p => p.InvoiceUid)
            .NotEmpty()
            .WithState(p => p);

        this.RuleFor(p => p.ItemQuantity)
            .GreaterThan(0)
            .WithState(p => p);
    }
}
