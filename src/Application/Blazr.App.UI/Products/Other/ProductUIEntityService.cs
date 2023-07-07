/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public sealed record ProductUIEntityService : IUIEntityService<ProductEntityService>
{
    public string SingleDisplayName { get; } = "Product";
    public string PluralDisplayName { get; } = "Products";
    public Type? EditForm { get; } = typeof(ProductEditForm);
    public Type? ViewForm { get; } = typeof(ProductViewForm);
    public string Url { get; } = "/product";
}
