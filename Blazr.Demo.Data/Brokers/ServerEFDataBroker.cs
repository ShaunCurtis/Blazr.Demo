/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public class ServerEFDataBroker<TDbContext>
    : IDataBroker
    where TDbContext : DbContext
{
    protected readonly IDbContextFactory<TDbContext> factory;
    private bool _success;
    private string? _message;

    public ServerEFDataBroker(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<RecordProviderResult<TRecord>> GetRecordAsync<TRecord>(Guid id) where TRecord : class, new()
    {
        var _dbContext = factory.CreateDbContext();

        TRecord? record = null;

        // first check if the record implements IRecord.  If so we can do a cast and then do the quesry via the Id property directly 
        if ((new TRecord()) is IRecord)
            record = await _dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Id == id);

        // Try and use the EF FindAsync implementation
        if (record == null)
            record = await _dbContext.FindAsync<TRecord>(id);

        if (record is null)
        {
            _message = "No record retrieved";
            _success = false;
        }

        return new RecordProviderResult<TRecord>(record, _success, _message);
    }

    public async ValueTask<RecordCountProviderResult> GetRecordCountAsync<TRecord>() where TRecord : class, new()
    {
        using var dbContext = factory.CreateDbContext();

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        var count = await query.CountAsync();

        return new RecordCountProviderResult(count);
    }

    public async ValueTask<FKListProviderResult> GetFKListAsync<TRecord>() where TRecord : class, IFkListItem, new()
    {
        using var dbContext = factory.CreateDbContext();

        IQueryable<TRecord> query = dbContext.Set<TRecord>();
        var list = await query.ToListAsync();

        if (list is null)
            return new FKListProviderResult(Enumerable.Empty<IFkListItem>(), false, "Coukld not retrieve the FK List");

        var fklist = list.Cast<IFkListItem>();

        return new FKListProviderResult(fklist);
    }

    public async ValueTask<CommandResult> AddRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
    {
        using var dbContext = factory.CreateDbContext();

        var id = GetRecordId<TRecord>(item);

        // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the record to
        dbContext.Add(item);

        // We should have added a single record so the return count should be 1
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(id, true, "Record Added")
            : new CommandResult(id, false, "Failed to Add Record");
    }

    public async ValueTask<CommandResult> UpdateRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
    {
        using var dbContext = factory.CreateDbContext();

        var id = GetRecordId<TRecord>(item);

        // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the record to
        dbContext.Update(item);

        // We should have added a single record so the return count should be 1
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(id, true, "Record Updated")
            : new CommandResult(id, false, "Failed to Update Record");
    }

    public async ValueTask<CommandResult> DeleteRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
    {
        using var dbContext = factory.CreateDbContext();

        var id = GetRecordId<TRecord>(item);

        // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the record to
        dbContext.Remove(item);

        // We should have added a single record so the return count should be 1
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(id, true, "Record Deleted")
            : new CommandResult(id, false, "Failed to Delete Record");
    }

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
    {
        _message = null;
        _success = true;
        var list = await this.GetItemsAsync<TRecord>(options);
        var count = await this.GetCountAsync<TRecord>(options);
        return new ListProviderResult<TRecord>(list, count, _success, _message);    
    }

    protected async ValueTask<IEnumerable<TRecord>> GetItemsAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
    {
        using var dbContext = factory.CreateDbContext();

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        if (!string.IsNullOrWhiteSpace(options.FilterExpression))
            query = query
                 .Where(options.FilterExpression);

        if (options.PageSize > 0)
            query = query
                .Skip(options.StartIndex)
                .Take(options.PageSize);

        try
        {
            return await query.ToListAsync();
        }
        catch
        {
            _success = false;
            _message = "Error in Executing Query.  This is probably caused by an incompatible SortExpression or QueryExpression";
            return new List<TRecord>();
        }
    }

    protected async ValueTask<int> GetCountAsync<TRecord>(ListProviderRequest options) where TRecord : class, new()
    {
        using var dbContext = factory.CreateDbContext();

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        if (!string.IsNullOrWhiteSpace(options.FilterExpression))
            query = query
                 .Where(options.FilterExpression);

        try
        {
            return await query.CountAsync();
        }
        catch
        {
            _success = false;
            _message = "Error in Executing Query.  This is probably caused by an incompatible SortExpression or QueryExpression";
            return 0;
        }
    }

    private static Guid GetRecordId<T>(T record) where T : class, new()
    {
        var instance = new T();
        var prop = instance.GetType()
            .GetProperties()
            .FirstOrDefault(prop => prop.GetCustomAttributes(false)
                .OfType<KeyAttribute>()
                .Any());

        if (prop != null)
        {
            var value = prop.GetValue(record);
            if (value is not null)
                return (Guid)value;
        }
        return Guid.Empty;
    }

}
