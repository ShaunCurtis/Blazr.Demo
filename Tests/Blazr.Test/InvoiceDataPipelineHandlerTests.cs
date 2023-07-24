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

public class InvoiceDataPipelineHandlerTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public InvoiceDataPipelineHandlerTests()
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
    public async void TestRepositoryDataBrokerDeleteInvoice()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var startInvoiceCount = _testDataProvider.InvoiceCount;
        var testInvoiceUid = _testDataProvider.TestInvoiceUid;

        var itemRequest = new ItemQueryRequest(testInvoiceUid, cancelToken);
        var itemResult = await broker!.GetItemAsync<Invoice>(itemRequest);

        Assert.True(itemResult.Successful);

        var deleteInvoice = itemResult.Item! with { StateCode = AppStateCodes.Delete };

        var command = new CommandRequest<Invoice>(deleteInvoice, cancelToken);
        var commandResult = await broker!.ExecuteCommandAsync<Invoice>(command);

        var listRequest = new ListQueryRequest() { StartIndex = 0, PageSize = 1000, Cancellation = cancelToken };
        var listResult = await broker!.GetItemsAsync<Invoice>(listRequest);

        Assert.False(commandResult.Successful);
        Assert.True(listResult.Successful);
        Assert.Equal(startInvoiceCount, listResult.TotalCount);
    }


    [Fact]
    public async void TestGetInvoiceAggregate()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        //Get a known root Uid
        var invoiceUid = _testDataProvider.TestInvoiceUid;

        var cancelToken = new CancellationToken();
        var itemRequest = new ItemQueryRequest { Uid = invoiceUid, Cancellation = cancelToken };

        var result = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        Assert.True(result.Successful);
        Assert.Equal(result.Item!.Uid, invoiceUid);
    }

    [Fact]
    public async void TestUpdateAggregateRoot()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        //Get a known root Uid
        var invoiceUid = _testDataProvider.TestInvoiceUid;

        var cancelToken = new CancellationToken();
        var itemRequest = new ItemQueryRequest { Uid = invoiceUid, Cancellation = cancelToken };

        var result = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        var aggregate = result.Item!;

        var aggregateRoot = result.Item!.Root with { };
        var modifiedAggregateRoot = aggregateRoot with { InvoiceNumber = $"{aggregateRoot.InvoiceNumber} - Modified at {DateTime.Now.ToLongTimeString()} " };

        var updateAggregateResult = aggregate.UpdateRoot(modifiedAggregateRoot);

        var command = new CommandRequest<InvoiceAggregate>() { Cancellation = cancelToken, Item = aggregate };
        var commandBrokerResult = await broker.ExecuteCommandAsync<InvoiceAggregate>(command);

        var queryBrokerResult = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        Assert.True(updateAggregateResult.Successful);
        Assert.True(commandBrokerResult.Successful);
        Assert.True(queryBrokerResult.Successful);
        Assert.Equal(modifiedAggregateRoot, queryBrokerResult.Item!.Root);
    }

    [Fact]
    public async void TestAddNewAggregate()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        var cancelToken = new CancellationToken();

        var customer = _testDataProvider.Customers.First();
        var product = _testDataProvider.Products.Skip(Random.Shared.Next(0, _testDataProvider.Products.Count() - 1)).First();

        // Create a fully populated new room item 
        var newRoot = InvoiceFactory.New(customer);

        // When we do a comparison with the saved root it's state will be 1
        var savedNewRoot = newRoot with { StateCode = 1 };

        // Create a new aggregate with the the new root and an empty collection
        var aggregate = new InvoiceAggregate(newRoot, Enumerable.Empty<InvoiceItem>());

        // Get the added root
        var aggregateRoot = aggregate.Root with { };

        var aggregateUid = aggregate.Uid;

        // Create a fully populated collection item
        var newCollectionItem = InvoiceFactory.New(newRoot, product, 2);

        // When we do a comparison with the saved root it's state will be 1
        var savedCollectionItem = newCollectionItem with { StateCode = 1 };

        var collectionItemSaveResult = aggregate.SaveCollectionItem(newCollectionItem);

        var command = new CommandRequest<InvoiceAggregate>() { Cancellation = cancelToken, Item = aggregate };
        var commandBrokerResult = await broker.ExecuteCommandAsync<InvoiceAggregate>(command);

        var itemRequest = new ItemQueryRequest { Uid = aggregateUid, Cancellation = cancelToken };

        var queryBrokerResult = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        var retrievedCollectionItemResult = queryBrokerResult.Item!.GetCollectionItem(new ItemQueryRequest { Uid = newCollectionItem.Uid });

        Assert.Equal(newRoot, aggregateRoot);
        Assert.True(collectionItemSaveResult.Successful);
        Assert.True(commandBrokerResult.Successful);
        Assert.True(queryBrokerResult.Successful);
        Assert.Equal(savedNewRoot, queryBrokerResult.Item!.Root);
        Assert.Single(queryBrokerResult.Item!.AllItems);
        Assert.True(retrievedCollectionItemResult.Successful);
        Assert.Equal(savedCollectionItem, retrievedCollectionItemResult.Item);
    }

    [Fact]
    public async void TestAddInvoiceItemToAggregate()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        //Get a known root Uid
        var invoiceUid = _testDataProvider.TestInvoiceUid;
        var product = _testDataProvider.Products.Skip(Random.Shared.Next(0, _testDataProvider.Products.Count() - 1)).First();

        var cancelToken = new CancellationToken();
        var itemRequest = new ItemQueryRequest { Uid = invoiceUid, Cancellation = cancelToken };

        var result = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        var aggregate = result.Item!;
        var root = aggregate.Root;

        // Create a fully populated collection item
        var newCollectionItem = InvoiceFactory.New(root, product, 2);

        // When we do a comparison with the saved root it's state will be 1
        var savedCollectionItem = newCollectionItem with { StateCode = 1 };

        // Save the item to the aggregate
        var collectionItemSaveResult = aggregate.SaveCollectionItem(newCollectionItem);

        // Persist the Aggregate to the data store
        var command = new CommandRequest<InvoiceAggregate>() { Cancellation = cancelToken, Item = aggregate };
        var commandBrokerResult = await broker.ExecuteCommandAsync<InvoiceAggregate>(command);

        // Get the saved aggregate
        var updatedResult = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);
        Assert.True(updatedResult.Successful);

        var addedItemRequest = new ItemQueryRequest { Uid = newCollectionItem.Uid, Cancellation = cancelToken };
        var retrievedCollectionItemResult = updatedResult.Item!.GetCollectionItem(addedItemRequest);

        Assert.True(collectionItemSaveResult.Successful);
        Assert.True(commandBrokerResult.Successful);
        Assert.True(retrievedCollectionItemResult.Successful);
        Assert.Equal(savedCollectionItem, retrievedCollectionItemResult.Item);
    }

    [Fact]
    public async void TestUpdateInvoiceItemInAggregate()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        //Get a known root Uid
        var invoiceUid = _testDataProvider.TestInvoiceUid;
        var product = _testDataProvider.Products.Skip(Random.Shared.Next(0, _testDataProvider.Products.Count() - 1)).First();

        var cancelToken = new CancellationToken();
        var itemRequest = new ItemQueryRequest { Uid = invoiceUid, Cancellation = cancelToken };

        var result = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        var aggregate = result.Item!;

        var collectionItem = aggregate.AllItems.First();
        var UpdateUid = collectionItem.Uid;

        // Update the item
        var updatedCollectionItem = collectionItem with { ItemQuantity = collectionItem.ItemQuantity + 2 };

        // Save the item to the aggregate
        var collectionItemSaveResult = aggregate.SaveCollectionItem(updatedCollectionItem);

        // Persist the Aggregate to the data store
        var command = new CommandRequest<InvoiceAggregate>() { Cancellation = cancelToken, Item = aggregate };
        var commandBrokerResult = await broker.ExecuteCommandAsync<InvoiceAggregate>(command);

        // Get the saved aggregate
        var updatedResult = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);
        Assert.True(updatedResult.Successful);

        var addedItemRequest = new ItemQueryRequest { Uid = UpdateUid, Cancellation = cancelToken };
        var retrievedCollectionItemResult = updatedResult.Item!.GetCollectionItem(addedItemRequest);

        Assert.True(collectionItemSaveResult.Successful);
        Assert.True(commandBrokerResult.Successful);
        Assert.True(retrievedCollectionItemResult.Successful);
        Assert.Equal(updatedCollectionItem, retrievedCollectionItemResult.Item);
    }

    [Fact]
    public async void TestDeleteInvoiceItemInAggregate()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IDataBroker>()!;

        //Get a known root Uid
        var invoiceUid = _testDataProvider.TestInvoiceUid;
        var product = _testDataProvider.Products.Skip(Random.Shared.Next(0, _testDataProvider.Products.Count() - 1)).First();

        var cancelToken = new CancellationToken();
        var itemRequest = new ItemQueryRequest { Uid = invoiceUid, Cancellation = cancelToken };

        var result = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        var aggregate = result.Item!;

        var collectionItem = aggregate.AllItems.First();
        var UpdateUid = collectionItem.Uid;
        var expectedCollectionCount = aggregate.LiveItems.Count() - 1;

        // Update the item
        var updatedCollectionItem = collectionItem with { StateCode = InvoiceStateCodes.Delete };

        // Save the item to the aggregate
        var collectionItemSaveResult = aggregate.SaveCollectionItem(updatedCollectionItem);

        // Persist the Aggregate to the data store
        var command = new CommandRequest<InvoiceAggregate>() { Cancellation = cancelToken, Item = aggregate };
        var commandBrokerResult = await broker.ExecuteCommandAsync<InvoiceAggregate>(command);

        // Get the saved aggregate
        var updatedResult = await broker!.GetItemAsync<InvoiceAggregate>(itemRequest);

        var addedItemRequest = new ItemQueryRequest { Uid = UpdateUid, Cancellation = cancelToken };
        var retrievedCollectionItemResult = updatedResult.Item!.GetCollectionItem(addedItemRequest);

        Assert.False(retrievedCollectionItemResult.Successful);
        Assert.Equal(expectedCollectionCount, updatedResult.Item!.AllItems.Count());
    }
}
