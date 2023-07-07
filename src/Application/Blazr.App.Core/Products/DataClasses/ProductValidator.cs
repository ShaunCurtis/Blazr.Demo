/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class ProductValidator : AbstractValidator<ProductEditContext>
{
    public ProductValidator()
    {
        this.RuleFor(p => p.ProductCode)
            .NotEmpty()
            .WithState(p => p);

        this.RuleFor(p => p.ProductName)
            .MinimumLength(3)
            .WithState(p => p);

        this.RuleFor(p => p.ProductUnitPrice)
            .GreaterThan(0)
            .WithState(p => p);
    }
}
