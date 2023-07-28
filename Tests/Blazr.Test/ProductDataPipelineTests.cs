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

public class ProductDataPipelineTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public ProductDataPipelineTests()
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
    public async void GetItem()
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
    public async void GetItems()
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
    public async void DeleteItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var originalCount = _testDataProvider.Products.Count();
        var expectedCount = originalCount - 1;
        var testItem = _testDataProvider.TestProduct;
        var testUid = testItem.Uid;

        var deleteItem = testItem with { EntityState = testItem.EntityState with { MarkedForDeletion = true } };

        var command = new CommandRequest<Product>(deleteItem, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Product>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Product>(listRequest);

        Assert.True(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(expectedCount, listResult.TotalCount);
    }

    [Fact]
    public async void AddItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var originalCount = _testDataProvider.Products.Count();
        var expectedCount = originalCount + 1;
        var newItem = new Product() { ProductCode = "SKU999", ProductName = "Test-Product", ProductUnitPrice = 20000, EntityState = new(StateCodes.New), ProductUid = new(Guid.NewGuid()) };
        var savedItem = newItem with { EntityState = new(StateCodes.Existing) };
        var productUid = newItem.Uid;

        var command = new CommandRequest<Product>(newItem, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Product>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Product>(listRequest);

        var itemRequest = new ItemQueryRequest(productUid, cancelToken);
        var itemResult = await broker!.GetItemAsync<Product>(itemRequest);

        Assert.True(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.True(itemResult.Successful);
        Assert.Equal(savedItem, itemResult.Item);
    }

    [Fact]
    public async void UpdateItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var originalCount = _testDataProvider.Products.Count();
        var expectedCount = originalCount;
        var testItem = _testDataProvider.TestProduct;

        var expectedItem = testItem with { ProductUnitPrice = 99999};
        var updatedItem = expectedItem with { EntityState = testItem.EntityState.Mutate() };
        var productUid = updatedItem.Uid;

        var command = new CommandRequest<Product>(updatedItem, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Product>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Product>(listRequest);

        var itemRequest = new ItemQueryRequest(productUid, cancelToken);
        var itemResult = await broker!.GetItemAsync<Product>(itemRequest);

        Assert.True(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.True(itemResult.Successful);
        Assert.Equal(expectedItem, itemResult.Item);
    }
}
