﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Infrastructure;

/// <summary>
/// This class provides item object mapping on top of the standard item handler
/// The out object is domain object
/// The in object is the Dbo object defining the record to retrieve from the data store
/// and should be defined in the DbContext
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <typeparam name="TDomainRecord">Domain Record to Map the incoming infrastructure record to</typeparam>
/// <typeparam name="TDatabaseRecord">This is the Dbo object the Handler will retrieve from the database</typeparam>
public sealed class MappedItemRequestServerHandler<TDbContext, TDomainRecord, TDatabaseRecord, TKey>
    : IItemRequestHandler<TDomainRecord, TKey>
    where TDbContext : DbContext
    where TDatabaseRecord : class
    where TDomainRecord : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public MappedItemRequestServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<ItemQueryResult<TDomainRecord>> ExecuteAsync(ItemQueryRequest<TKey> request)
    {
        return await this.GetItemAsync(request);
    }

    private async ValueTask<ItemQueryResult<TDomainRecord>> GetItemAsync(ItemQueryRequest<TKey> request)
    {
        // Get and check we have a mapper for the Dbo object to Dco Domain Model
        IDboEntityMap<TDatabaseRecord, TDomainRecord>? mapper = null;
        mapper = _serviceProvider.GetService<IDboEntityMap<TDatabaseRecord, TDomainRecord>>();

        // Throw an exception if we have no mapper defined 
        if (mapper is null)
            throw new DataPipelineException($"No mapper is defined for {this.GetType().FullName} for {(typeof(TDatabaseRecord).FullName)}");

        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var inRecord = await dbContext.Set<TDatabaseRecord>().FindAsync(request.KeyValue, request.Cancellation);

        if (inRecord is null)
            return ItemQueryResult<TDomainRecord>.Failure($"No record retrieved with a Uid of {request.KeyValue?.ToString()}");

        var outRecord = mapper.MapTo(inRecord);

        if (outRecord is null)
            return ItemQueryResult<TDomainRecord>.Failure($"Unable to map record retrieved with a Uid of {request.KeyValue?.ToString()}");

        return ItemQueryResult<TDomainRecord>.Success(outRecord);
    }
}