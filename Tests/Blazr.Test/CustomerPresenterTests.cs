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

        var request = new ListQueryRequest() { PageSize=1 };
        var testCount = _testDataProvider.Customers.Count();
        await presenter.GetItemsAsync(request);

        Assert.Equal(testCount, presenter.Item);
    }

}
