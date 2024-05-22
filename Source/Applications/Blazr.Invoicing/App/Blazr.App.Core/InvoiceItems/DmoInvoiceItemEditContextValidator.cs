/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class DmoInvoiceItemEditContextValidator : AbstractValidator<DmoInvoiceItemEditContext>
{
    public DmoInvoiceItemEditContextValidator()
    {
        this.RuleFor(p => p.Description)
            .MinimumLength(3)
            .WithState(p => p);

        this.RuleFor(p => p.Amount)
            .GreaterThanOrEqualTo(0)
            .WithState(p => p);
    }
}
