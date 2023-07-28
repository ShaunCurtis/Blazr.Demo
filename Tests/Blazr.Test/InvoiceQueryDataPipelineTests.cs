/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazr.Test;

public class InvoiceQueryDataPipelineTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public InvoiceQueryDataPipelineTests()
        => _testDataProvider = InvoiceTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddAppTestInfrastructureServices();
        services.AddAppPresentationServices();
        services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        // get the DbContext factory and add the test data
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceDbContext>>();
        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceDbContext>(factory);

        return provider!;
    }

    [Theory]
    [InlineData(10, 0, 10)]
    [InlineData(10, 20, 4)]
    [InlineData(100, 0, 24)]
    public async void GetPagedProductItems(int pageSize, int startIndex, int expected)
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var actualCount = _testDataProvider.Products.Count();
        var listRequest = new ListQueryRequest() { StartIndex = startIndex, PageSize = pageSize, Cancellation = cancelToken };
        var result = await broker!.GetItemsAsync<Product>(listRequest);

        Assert.True(result.Successful);
        Assert.Equal(expected, result.Items.Count());
        Assert.Equal(actualCount, result.TotalCount);
    }

    [Fact]
    public async void GetFilteredPagedInvoiceItems()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();
        var testCustomerUid = _testDataProvider.TestCustomerUid;
        var actualCount = _testDataProvider.CustomerInvoiceCount(testCustomerUid);
        var filter = new FilterDefinition(ApplicationConstants.Invoice.FilterByCustomerUid, testCustomerUid.ToString());
        var filters = new List<FilterDefinition>() { filter };
        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken, Filters = filters };
        var result = await broker!.GetItemsAsync<Invoice>(listRequest);

        Assert.True(result.Successful);
        Assert.Equal(actualCount, result.Items.Count());
        Assert.Equal(actualCount, result.TotalCount);
    }

    [Fact]
    public async void TestGetSortedPagedProductItems()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();
        var manufacturer = "Fokker";
        var firstItem = _testDataProvider.FirstManufacturersProduct(manufacturer);

        var sorter = new SortDefinition(ApplicationConstants.Product.ProductName, false);
        var sorters = new List<SortDefinition>() { sorter };

        var filter = new FilterDefinition(ApplicationConstants.Product.FilterByManufacturerName, manufacturer);
        var filters = new List<FilterDefinition>() { filter };

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken, Sorters = sorters, Filters = filters };
        var result = await broker!.GetItemsAsync<Product>(listRequest);

        var firstReturnedItem = result.Items?.FirstOrDefault();
        Assert.True(result.Successful);
        Assert.Equal(firstItem, firstReturnedItem);
    }


    [Fact]
    public async void TestGetSortedAndFilteredPagedProductItems()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();
        var firstItem = _testDataProvider.FirstProduct;

        var sorter = new SortDefinition(ApplicationConstants.Product.ProductName, false);
        var sorters = new List<SortDefinition>() { sorter };

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken, Sorters = sorters };
        var result = await broker!.GetItemsAsync<Product>(listRequest);

        var firstReturnedItem = result.Items?.FirstOrDefault();
        Assert.True(result.Successful);
        Assert.Equal(firstItem, firstReturnedItem);
    }
}
