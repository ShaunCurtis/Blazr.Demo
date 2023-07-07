/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class ProductsByManufacturerSpecification : PredicateSpecification<Product>
{
    private string _manufacturer;

    public ProductsByManufacturerSpecification(string manufacturer)
    {
       _manufacturer = manufacturer;
    }

    public ProductsByManufacturerSpecification(FilterDefinition filter)
        => _manufacturer = filter.FilterData;

    public override Expression<Func<Product, bool>> Expression
        => item => item.ProductName.Contains(_manufacturer);
}
