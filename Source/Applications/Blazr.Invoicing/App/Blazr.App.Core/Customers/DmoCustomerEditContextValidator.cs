/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class DmoCustomerEditContextValidator : AbstractValidator<CustomerEditContext>
{
    public DmoCustomerEditContextValidator()
    {
        this.RuleFor(p => p.CustomerName)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
