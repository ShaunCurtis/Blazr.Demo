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
public interface IListService<TRecord>
    where TRecord : class, new()
{
    public int PageSize { get;}
    public int StartIndex { get; }
    public int ListCount { get; }
    public IEnumerable<TRecord>? Records { get; }
    public string? Message { get; }
    public bool IsPaging => (PageSize > 0);
    public bool HasList => this.Records is not null;

    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(int startRecord, int pageSize);

    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request);

    public ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest request);
}
```
### Standard Implementation

The standard

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

    public async ValueTask GetRecordAsync(Guid Id)
    {
        this.Message = String.Empty;
        var result = await this.DataBroker.ExecuteAsync<TRecord>(new RecordGuidKeyQuery<TRecord>(Id));

        if (result.Success && result.Record is not null)
        {
            this.Record = result.Record;
            return;
        }
        this.Record = null;
        this.Message = $"Failed to retrieve the record with Id - {Id.ToString()}";
    }

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request)
    {
        this.Message = String.Empty;
        this.PageSize = request.PageSize;
        this.StartIndex = request.StartIndex;

        var result = await this.DataBroker.ExecuteAsync<TRecord>( new RecordListQuery<TRecord>(request));

        if (result.Success && result.Items is not null)
        {
            this.Records = result.Items;
            this.ListCount = result.TotalItemCount;
            var page = request.StartIndex <= 0
                ? 0
                : (int)(request.StartIndex / request.PageSize);

            this.Notifier.NotifyListPaged(this, page);
        }
        else
        {
            this.Records = null;
            this.ListCount = 0;
            this.Message = $"Failed to retrieve the recordset at index {request.StartIndex}";
        }

        return new ListProviderResult<TRecord>(this.Records ?? Enumerable.Empty<TRecord>(), this.ListCount, result.Success, Message);
    }

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
}
```

