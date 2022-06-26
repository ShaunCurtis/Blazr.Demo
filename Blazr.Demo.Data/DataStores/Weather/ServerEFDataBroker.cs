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
    protected readonly IDbContextFactory<TDbContext> database;
    private bool _success;
    private string? _message;

    public ServerEFDataBroker(IDbContextFactory<TDbContext> db)
        => this.database = db;

    public async ValueTask<RecordProviderResult<TRecord>> GetRecordAsync<TRecord>(Guid id) where TRecord : class, new()
    {
        using var context = database.CreateDbContext();

        var key = GetKeyProperty<TRecord>();

        var dbSet = context.Set<TRecord>().AsNoTracking();

        if (dbSet is null)
            return new RecordProviderResult<TRecord> (null, false, "No DbSet found for the provided data class");

        var records = await dbSet
            .Where($"{key} == \"{id}\"")
            .ToListAsync();

        return records is not null && records.Count == 1
            ? new RecordProviderResult<TRecord>(records[0], true, "Record retrieved successfully.")
            : new RecordProviderResult<TRecord>(null, false, "Could not find the record in the DbSet."); 
    }

    public async ValueTask<RecordCountProviderResult> GetRecordCountAsync<TRecord>() where TRecord : class, new()
    {
        using var dbContext = database.CreateDbContext();

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        var count = await query.CountAsync();

        return new RecordCountProviderResult(count);
    }

    public async ValueTask<CommandResult> AddRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
    {
        using var dbContext = database.CreateDbContext();

        var id = GetRecordId<TRecord>(item);

        // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the rcord to
        dbContext.Add(item);

        // We should have added a single record so the return count should be 1
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(id, true, "Record Added")
            : new CommandResult(id, false, "Failed to Add Record");
    }

    public async ValueTask<CommandResult> UpdateRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
    {
        using var dbContext = database.CreateDbContext();

        var id = GetRecordId<TRecord>(item);

        // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the rcord to
        dbContext.Update(item);

        // We should have added a single record so the return count should be 1
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(id, true, "Record Updated")
            : new CommandResult(id, false, "Failed to Update Record");
    }

    public async ValueTask<CommandResult> DeleteRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
    {
        using var dbContext = database.CreateDbContext();

        var id = GetRecordId<TRecord>(item);

        // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the rcord to
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
        using var dbContext = database.CreateDbContext();

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
        using var dbContext = database.CreateDbContext();

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

    private static string GetKeyProperty<T>() where T : class, new()
    {
        var instance = new T();
        var prop = instance.GetType()
            .GetProperties()
            .FirstOrDefault(prop => prop.GetCustomAttributes(false)
                .OfType<KeyAttribute>()
                .Any());
        return prop?.Name ?? string.Empty;
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
