/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.Core;
using Blazr.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazr.Test;

public class CustomerPresenterTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public CustomerPresenterTests()
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

    [Fact]
    public async void GetItem()
    {
        var provider = GetServiceProvider();
        var presenter = provider.GetService<IReadPresenter<Customer>>()!;

        var testItem = _testDataProvider.TestCustomer;
        var testUid = testItem.Uid;
        await presenter.LoadAsync(testUid);

        Assert.Equal(testItem, presenter.Item);
    }

    [Fact]
    public async void GetItems()
    {
        var provider = GetServiceProvider();
        var presenter = provider.GetService<IListPresenter<Customer, CustomerEntityService>>()!;

        var request = new ListQueryRequest() { PageSize = 2 };
        var totalCount = _testDataProvider.Customers.Count();
        await presenter.GetItemsAsync(request);

        Assert.Equal(totalCount, presenter.ListController.ListState.ListTotalCount);
        Assert.Equal(2, presenter.ListController.Count());
    }

    [Fact]
    public async void AddItem()
    {
        var provider = GetServiceProvider();
        var presenter = provider.GetService<IBlazrEditPresenter<Customer, CustomerEditContext>>()!;
        var broker = provider.GetService<IDataBroker>()!;

        var expectedCount = _testDataProvider.Customers.Count();
        //var testItem = new Customer { CustomerUid = new(Guid.NewGuid()), CustomerName = "Dan Air", EntityState= Blazr.Core.EntityState.New};
        //var testUid = testItem.Uid;

        var expectedItem = testItem with { EntityState = Blazr.Core.EntityState.Existing };

        await presenter.LoadAsync(EntityUid.Empty);
        presenter.RecordContext.CustomerName = "Dan Air";

        await presenter.SaveItemAsync();

        var listRequest = new ListQueryRequest();
        var listResult = await broker!.GetItemsAsync<Customer>(listRequest);

        var itemRequest = new ItemQueryRequest(testUid);
        var itemResult = await broker!.GetItemAsync<Customer>(itemRequest);

        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.Equal(expectedItem, itemResult.Item);
    }

    [Fact]
    public async void UpdateItem()
    {
        var provider = GetServiceProvider();
        var presenter = provider.GetService<IBlazrEditPresenter<Customer, CustomerEditContext>>()!;
        var broker = provider.GetService<IDataBroker>()!;

        var expectedCount = _testDataProvider.Customers.Count();
        var testItem = _testDataProvider.TestCustomer;
        var testUid = testItem.Uid;

        var newCustomerName = testItem.CustomerName + " - Testing";
        var expectedItem = testItem with { CustomerName = newCustomerName };

        await presenter.LoadAsync(testUid);
        presenter.RecordContext.CustomerName = newCustomerName;

        await presenter.SaveItemAsync();

        var listRequest = new ListQueryRequest();
        var listResult = await broker!.GetItemsAsync<Customer>(listRequest);

        var itemRequest = new ItemQueryRequest(testUid);
        var itemResult = await broker!.GetItemAsync<Customer>(itemRequest);

        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.Equal(expectedItem, itemResult.Item);
    }

    [Fact]
    public async void DeleteItem()
    {
        var provider = GetServiceProvider();
        var presenter = provider.GetService<IBlazrEditPresenter<Customer, CustomerEditContext>>()!;
        var broker = provider.GetService<IDataBroker>()!;

        var expectedCount = _testDataProvider.Customers.Count() - 1;
        var testItem = _testDataProvider.TestCustomer;
        var testUid = testItem.Uid;

        await presenter.LoadAsync(testUid);

        presenter.RecordContext.SetAsDeleted();

        await presenter.SaveItemAsync();

        var listRequest = new ListQueryRequest();
        var listResult = await broker!.GetItemsAsync<Customer>(listRequest);

        var itemRequest = new ItemQueryRequest(testUid);
        var itemResult = await broker!.GetItemAsync<Customer>(itemRequest);

        Assert.Equal(expectedCount, listResult.TotalCount);
        Assert.False(itemResult.Successful);
    }
}
