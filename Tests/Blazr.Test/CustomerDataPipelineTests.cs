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

public class CustomerDataPipelineTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public CustomerDataPipelineTests()
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

        var testItem = _testDataProvider.TestCustomer;
        var testUid = testItem.Uid;
        var itemRequest = new ItemQueryRequest(testUid, cancelToken);
        var result = await broker!.GetItemAsync<Customer>(itemRequest);

        Assert.True(result.Successful);
        Assert.Equal(testItem, result.Item);
    }

    [Fact]
    public async void GetItems()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var actualCount = _testDataProvider.Customers.Count();
        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var result = await broker!.GetItemsAsync<Customer>(listRequest);

        Assert.True(result.Successful);
        Assert.Equal(actualCount, result.Items.Count());
    }

    [Fact]
    public async void DeleteItem()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var originalCount = _testDataProvider.Customers.Count();
        var expectedCount = originalCount - 1;
        var testItem = _testDataProvider.TestCustomer;
        var testUid = testItem.Uid;

        var deleteItem = testItem with { EntityState = testItem.EntityState with { MarkedForDeletion = true } };

        var command = new CommandRequest<Customer>(deleteItem, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Customer>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Customer>(listRequest);

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

        var originalCount = _testDataProvider.Customers.Count();
        var expectedCount = originalCount + 1;
        var newItem = new Customer() { CustomerName="Laker Airways" , EntityState = new(StateCodes.New), CustomerUid = new(Guid.NewGuid()) };
        var savedItem = newItem with { EntityState = new(StateCodes.Existing) };
        var Uid = newItem.Uid;

        var command = new CommandRequest<Customer>(newItem, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Customer>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Customer>(listRequest);

        var itemRequest = new ItemQueryRequest(Uid, cancelToken);
        var itemResult = await broker!.GetItemAsync<Customer>(itemRequest);

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

        var originalCount = _testDataProvider.Customers.Count();
        var expectedCount = originalCount;
        var testItem = _testDataProvider.TestCustomer;

        var expectedItem = testItem with { CustomerName = testItem.CustomerName +" - Test" };
        var updatedItem = expectedItem with { EntityState = testItem.EntityState.Mutate() };
        var Uid = updatedItem.Uid;

        var command = new CommandRequest<Customer>(updatedItem, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Customer>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Customer>(listRequest);

        var itemRequest = new ItemQueryRequest(Uid, cancelToken);
        var itemResult = await broker!.GetItemAsync<Customer>(itemRequest);

        Assert.True(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.True(itemResult.Successful);
        Assert.Equal(expectedItem, itemResult.Item);
    }
}
