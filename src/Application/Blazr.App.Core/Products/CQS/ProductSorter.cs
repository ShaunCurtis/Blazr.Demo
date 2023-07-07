/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class ProductSorter : RecordSorter<Product>
{
    protected override Expression<Func<Product, object>> DefaultSorter => item => item.ProductCode ?? string.Empty;
}
