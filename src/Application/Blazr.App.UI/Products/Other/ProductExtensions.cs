/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public static class ProductExtensions
{
    public static string PriceAsCurrency(this Product item)
        => $"£{item.ProductUnitPrice.ToString("N2")}";
}
