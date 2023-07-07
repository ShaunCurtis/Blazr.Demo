/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class ProductPresentationServices
{
    public static void AddProductPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<ProductEntityService>();
        services.AddScoped<IListPresenter<Product, ProductEntityService>, ListPresenter<Product, ProductEntityService>>();
        services.AddScoped<INotificationService<ProductEntityService>, NotificationService<ProductEntityService>>();
        services.AddScoped<IForeignKeyPresenter<ProductFkItem, ProductEntityService>, ForeignKeyPresenter<ProductFkItem, ProductEntityService>>();
        services.AddTransient<IBlazrEditPresenter<Product, ProductEditContext>, BlazrEditPresenter<Product, ProductEntityService, ProductEditContext>>();
        services.AddTransient<IReadPresenter<Product>, ReadPresenter<Product>>();
        services.AddTransient<IListController<Product>, ListController<Product>>();
        services.AddTransient<IRecordSorter<Product>, ProductSorter>();
        services.AddTransient<IRecordFilter<Product>, ProductFilter>();
    }
}
