/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Infrastructure;

public sealed class MappedCommandServerHandler<TDbContext, TDomainRecord, TDatabaseRecord>
    : ICommandHandler<TDomainRecord>
    where TDbContext : DbContext
    where TDatabaseRecord : class
    where TDomainRecord : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public MappedCommandServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<TDomainRecord> request)
    {
        return await this.ExecuteCommandAsync(request);
    }

    private async ValueTask<CommandResult> ExecuteCommandAsync(CommandRequest<TDomainRecord> request)
    {
        // Check if command operations are allowed on the TDomainRecord object
        if ((request.Item is not ICommandEntity))
            return CommandResult.Failure($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly.");

        // Check we have a mapper for converting the TDomainRecord domain object to TDomainRecord DbContext object
        IDboEntityMap<TDatabaseRecord, TDomainRecord>? mapper = null;
        mapper = _serviceProvider.GetService<IDboEntityMap<TDatabaseRecord, TDomainRecord>>();

        if (mapper is null)
            return CommandResult.Failure($"No mapper defined for {this.GetType().FullName} for {(typeof(TDatabaseRecord).FullName)}");

        var dboRecord = mapper.MapTo(request.Item);

        using var dbContext = _factory.CreateDbContext();

        string success = "Action completed";
        string failure = $"Nothing executed.  Unrecognised State.";
        int recordsAffected = 0;
        bool isAdd = false;

        // First check if it's new.
        if (request.State == CommandState.Add)
        {
            success = "Record Added";
            failure = "Error Adding Record";
            isAdd = true;

            dbContext.Add<TDatabaseRecord>(dboRecord);
            recordsAffected = await dbContext.SaveChangesAsync(request.Cancellation);
        }

        // Check if we should delete it
        if (request.State == CommandState.Delete)
        {
            success = "Record Deleted";
            failure = "Error Deleting Record";

            dbContext.Remove<TDatabaseRecord>(dboRecord);
            recordsAffected = await dbContext
                .SaveChangesAsync(request.Cancellation)
                .ConfigureAwait(ConfigureAwaitOptions.None);
        }

        // Finally check if it's a update
        if (request.State == CommandState.Update)
        {
            success = "Record Updated";
            failure = "Error Updating Record";

            dbContext.Update<TDatabaseRecord>(dboRecord);
            recordsAffected = await dbContext
                .SaveChangesAsync(request.Cancellation)
                .ConfigureAwait(ConfigureAwaitOptions.None);
        }

        // We will have either 1 or 0 changed records
        if (recordsAffected == 1)
        {
            var isKeyed = dboRecord is IKeyedEntity;

            // Check if we need to return a database inserted key value
            if (isKeyed && isAdd)
            {
                var key = ((IKeyedEntity)dboRecord).KeyValue;
                return CommandResult.SuccessWithKey(key, success);
            }
            else
                return CommandResult.Success(success);
        }

        return CommandResult.Failure(failure);
    }
}
