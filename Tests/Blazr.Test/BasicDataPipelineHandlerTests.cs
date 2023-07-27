/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazr.Test;

public class BasicDataPipelineHandlerTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public BasicDataPipelineHandlerTests()
        => _testDataProvider = InvoiceTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddAppTestInfrastructureServices();
        services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        // get the DbContext factory and add the test data
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceDbContext>>();
        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceDbContext>(factory);

        return provider!;
    }

    [Fact]
    public async void TestRepositoryDataBrokerGetProductItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var testProduct = _testDataProvider.TestProduct;
        var testUid = testProduct.Uid;
        var itemRequest = new ItemQueryRequest(testUid, cancelToken);
        var result = await broker!.GetItemAsync<Product>(itemRequest);

        Assert.True(result.Successful);
        Assert.Equal(testProduct, result.Item);
    }

    [Fact]
    public async void TestRepositoryDataBrokerGetProductItems()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var actualCount = _testDataProvider.Products.Count();
        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var result = await broker!.GetItemsAsync<Product>(listRequest);

        Assert.True(result.Successful);
        Assert.Equal(actualCount, result.Items.Count());
    }


    [Fact]
    public async void TestRepositoryDataBrokerDeleteProductItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var originalCount = _testDataProvider.Products.Count();
        var expectedCount = originalCount - 1;
        var testProduct = _testDataProvider.TestProduct;
        var testUid = testProduct.Uid;

        var deleteProduct = testProduct with { EntityState = testProduct.EntityState with { MarkedForDeletion=true } };

        var command = new CommandRequest<Product>(deleteProduct, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Product>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Product>(listRequest);

        Assert.True(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(expectedCount, listResult.TotalCount);
    }

    [Fact]
    public async void TestRepositoryDataBrokerAddProductItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var originalCount = _testDataProvider.Products.Count();
        var expectedCount = originalCount + 1;
        var newProduct = new Product() { ProductCode = "SKU999", ProductName = "Test-Product", ProductUnitPrice = 20000, EntityState = new(StateCodes.New), ProductUid = new(Guid.NewGuid()) };
        var savedProduct = newProduct with { EntityState = new(StateCodes.Existing) };
        var productUid = newProduct.Uid;

        var command = new CommandRequest<Product>(newProduct, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Product>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Product>(listRequest);

        var itemRequest = new ItemQueryRequest(productUid, cancelToken);
        var itemResult = await broker!.GetItemAsync<Product>(itemRequest);

        Assert.True(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.True(itemResult.Successful);
        Assert.Equal(savedProduct, itemResult.Item);
    }

    [Fact]
    public async void TestRepositoryDataBrokerUpdateProductItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var originalCount = _testDataProvider.Products.Count();
        var expectedCount = originalCount;
        var testProduct = _testDataProvider.TestProduct;

        var updatedProduct = testProduct with { ProductUnitPrice = 99999 };
        var productUid = updatedProduct.Uid;

        var command = new CommandRequest<Product>(updatedProduct, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Product>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Product>(listRequest);

        var itemRequest = new ItemQueryRequest(productUid, cancelToken);
        var itemResult = await broker!.GetItemAsync<Product>(itemRequest);

        Assert.True(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.True(itemResult.Successful);
        Assert.Equal(updatedProduct, itemResult.Item);
    }

    [Theory]
    [InlineData(10, 0, 10)]
    [InlineData(10, 20, 4)]
    [InlineData(100, 0, 24)]
    public async void TestRepositoryDataBrokerGetPagedProductItems(int pageSize, int startIndex, int expected)
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
    public async void TestRepositoryDataBrokerGetFilteredPagedInvoiceItems()
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
    public async void TestRepositoryDataBrokerGetSortedPagedProductItems()
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
    public async void TestRepositoryDataBrokerGetSortedAndFilteredPagedProductItems()
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
