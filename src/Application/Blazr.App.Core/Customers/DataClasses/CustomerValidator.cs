/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class CustomerValidator : AbstractValidator<CustomerEditContext>
{
    public CustomerValidator()
    {
        this.RuleFor(p => p.CustomerName)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
