/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class InvoiceValidator : AbstractValidator<InvoiceEditContext>
{
    public InvoiceValidator()
    {
        this.RuleFor(p => p.CustomerUid)
            .NotEmpty()
            .WithState(p => p);

        this.RuleFor(p => p.InvoiceDate)
            .NotEmpty()
            .WithState(p => p);
    }
}
