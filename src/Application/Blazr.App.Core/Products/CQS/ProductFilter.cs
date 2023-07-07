/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class ProductFilter : IRecordFilter<Product>
{
    public IPredicateSpecification<Product>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            ApplicationConstants.Product.FilterByManufacturerName => new ProductsByManufacturerSpecification(filter),
            _ => null
        };
}
