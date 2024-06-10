﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

/// <summary>
/// This class provides item object mapping on top of the standard item handler
/// The out object is domain object
/// The in object is the Dbo object defining the record to retrieve from the data store
/// and should be defined in the DbContext
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public sealed class InvoiceRequestServerHandler<TDbContext>
    : IItemRequestHandler<DmoInvoice>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public InvoiceRequestServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<ItemQueryResult<DmoInvoice>> ExecuteAsync(ItemQueryRequest request)
    {
        return await this.GetItemAsync(request);
    }

    private async ValueTask<ItemQueryResult<DmoInvoice>> GetItemAsync(ItemQueryRequest request)
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        DvoInvoice? inRecord = null;

        if (request.KeyValue is IGuidKey keyId)
            inRecord = await dbContext.Set<DvoInvoice>().FirstOrDefaultAsync(item => item.InvoiceID == keyId.Value);

        if (request.KeyValue is Guid uid)
            inRecord = await dbContext.Set<DvoInvoice>().FirstOrDefaultAsync(item => item.InvoiceID == uid);

        if (inRecord is null)
            return ItemQueryResult<DmoInvoice>.Failure($"No record retrieved with a Id of {request.KeyValue.ToString()}");

        var outRecord = DvoInvoiceMap.Map(inRecord);

        if (outRecord is null)
            return ItemQueryResult<DmoInvoice>.Failure($"Unable to map record retrieved with a Id of {request.KeyValue.ToString()}");

        return ItemQueryResult<DmoInvoice>.Success(outRecord);
    }
}