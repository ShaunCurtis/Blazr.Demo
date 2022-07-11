# View Services

## Entities

Services are based on *Entities*:  in the Blazor template *WeatherForecast* is an entity.

For each entity there are four, with one or more optional, view services:

1. ListServices - handles getting collections of items.
2. RecordServices - handles getting indivual records.
3. EditServices - handles CRUD operartions on a record.
4. EntityService - handles custom operations associated with an entity.
5. FKServices - handles foreign Key operstions where the entity is used as a foreign key in other entities.

There's also an `Entity` class, derived from `IEntity`. for each entity to act as a unique identifier for the entity in DI service instances.  We'll see this in action later.

The services are split this way to obey the 'S' of the SOLID Principles- *Single Reposibility Principle*.

## Generics

Our interfaces and base classes use generics.  They define:
1. The record as `TRecord` and qualify it as  `class. new()`.
2. The Entity object as `TEntity` to qualify the type in DI service definitions, 

## List Services

The purpose of the List Services is to provide a `IEnumerable` collection of `TRecord.`

### Interface

```csharp
public interface IListService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    public int PageSize { get;}
    public int StartIndex { get; }
    public int ListCount { get; }
    public IEnumerable<TRecord>? Records { get; }
    public string? Message { get; }
    public bool IsPaging => (PageSize > 0);
    public bool HasList => this.Records is not null;

    public void SetNotificationService(INotificationService<TEntity> service);
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(int startRecord, int pageSize);
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request);
    public ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest request);
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(IFilteredListQuery<TRecord> query);
}
```
### Standard Implementation

This is the standard implementation.  There are several versions of `GetRecordsAsync` based on input which make it long.

```csharp
public class StandardListService<TRecord, TEntity>
    : IListService<TRecord>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected ICQSDataBroker DataBroker;
    protected INotificationService<TEntity> Notifier;
    public int PageSize { get; protected set; }
    public int StartIndex { get; protected set; }
    public int ListCount { get; protected set; }
    public IEnumerable<TRecord>? Records { get; private set; }
    public TRecord? Record { get; private set; }
    public string? Message { get; protected set; }
    public bool IsPaging => (PageSize > 0);
    public bool HasList => this.Records is not null;
    public bool HasRecord => this.Record is not null;

    public StandardListService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier)
    {
        this.DataBroker = dataBroker;
        Notifier = notifier;
    }

    public void SetNotificationService(INotificationService<TEntity> service)
        => this.Notifier = service;

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(int startRecord, int pageSize)
    {
        this.StartIndex = startRecord;
        this.PageSize = pageSize;

        return await this.GetRecordsAsync();
    }

    public async ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest itemsRequest)
    {
        var request = new ListProviderRequest<TRecord>(itemsRequest);
        this.StartIndex = request.StartIndex;
        this.PageSize = this.PageSize;

        await this.GetRecordsAsync(request);

        return new ItemsProviderResult<TRecord>(this.Records ?? new List<TRecord>(), this.ListCount);
    }

    private async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync()
    {
        var cancel = new CancellationToken();
        var request = new ListProviderRequest<TRecord>(this.StartIndex, this.PageSize, cancel);

        return await this.GetRecordsAsync(request);
    }
    
    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request)
    {
        this.Message = String.Empty;
        this.PageSize = request.PageSize;
        this.StartIndex = request.StartIndex;

        var query = new RecordListQuery<TRecord>(request);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(query);

        return this.GetResult(result);
    }

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(IFilteredListQuery<TRecord> query)
    {
        this.Message = String.Empty;
        this.PageSize = query.Request.PageSize;
        this.StartIndex = query.Request.StartIndex;

        var result = await this.DataBroker.ExecuteAsync<TRecord>(query);

        return this.GetResult(result);
    }

    private ListProviderResult<TRecord> GetResult(ListProviderResult<TRecord> result)
    {

        if (result.Success && result.Items is not null)
        {
            this.Records = result.Items;
            this.ListCount = result.TotalItemCount;
            var page = this.StartIndex <= 0
                ? 0
                : (int)(this.StartIndex / this.PageSize);

            this.Notifier.NotifyListPaged(this, page);
        }
        else
        {
            this.Records = null;
            this.ListCount = 0;
            this.Message = $"Failed to retrieve the recordset at index {this.StartIndex}";
        }

        return new ListProviderResult<TRecord>(this.Records ?? Enumerable.Empty<TRecord>(), this.ListCount, result.Success, Message);
    }
}
```

`SetNotificationService` is a bit unique.  It's used when the ListService is set in an `OwningComponentBase` form and the notification service needs to be set to the top level instance of the `INotificationService` instance rather then the one created in the `OwningComponentBase` service container. 

