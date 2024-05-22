/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class DmoInvoiceEditContextValidator : AbstractValidator<DmoInvoiceEditContext>
{
    public DmoInvoiceEditContextValidator()
    {
        this.RuleFor(p => p.Customer)
            .NotNull()
            .WithState(p => p);

        this.RuleFor(p => p.Date)
            .NotNull()
            .WithState(p => p);
    }
}
