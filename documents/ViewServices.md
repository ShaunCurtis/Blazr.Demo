# View Services

> **Work in progress**

## Entities

Services are based on *Entities*: for example, *WeatherForecast* is an entity.

For each entity there are four principle, and one or more optional, view services:

1. ListServices - handles getting collections of items.
2. RecordServices - handles getting indivual records.
3. EditServices - handles CRUD operartions on a record.
4. EntityService - handles custom operations associated with an entity.
5. FKServices - handles foreign Key operstions where the entity is used as a foreign key in other entities.

Each entity has an empty `Entity` class, derived from `IEntity`, that axts as it's unique identifier when defining service class instances for Dependancy Injection.   We'll see this in action later.

## Generics

Our interfaces and base classes use generics.  They define:
1. The record as `TRecord` and qualify it as  `class. new()`.
2. The Entity object as `TEntity` qualified as implementing `IEntity`. 

### Base View Service

`BaseViewService` contains all the boilerplate code used by all the different record services.

This is:

1. The Data Broker
2. The Entity Notification Service
3. The Authentication and Authorization Services
4. `SetServices` to set the View Service injected serives to the correct instances of these services
5. Two Check authentication methods.

if the view service is used in a standard `ComponentBase` context then `SetServices` is not required.  The services are already set to the SPA scoped service instances.  However, if the view service is used in the `OwningComponentBase` context, then the other services will be new instances created in the `OwningComponentBase` DI container.  To solve this problem we call `SetServices` on rthe view services and pass in the reference to the SPA scoped service container.  It then sets the relevant services to the correct instances. 

```csharp
public class BaseViewService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected ICQSDataBroker DataBroker;
    protected INotificationService<TEntity> Notifier;
    protected AuthenticationStateProvider AuthenticationStateProvider;
    protected IAuthorizationService AuthorizationService;

    public string? Message { get; protected set; }

    public BaseViewService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
    {
        this.DataBroker = dataBroker;
        this.Notifier = notifier;
        this.AuthenticationStateProvider = authenticationState;
        this.AuthorizationService = authorizationService;
    }

    public void SetServices(IServiceProvider services)
    {
        this.Notifier = services.GetService(typeof(INotificationService<TEntity>)) as INotificationService<TEntity> ?? default!;
        this.AuthenticationStateProvider = services.GetService(typeof(AuthenticationStateProvider)) as AuthenticationStateProvider ?? default!;
        this.AuthorizationService = services.GetService(typeof(IAuthorizationService)) as IAuthorizationService ?? default!;
    }

    protected async ValueTask<bool> CheckAuthorization(string policyName)
    {
        var authstate = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        var result = await this.AuthorizationService.AuthorizeAsync(authstate.User, null, policyName);

        if (!result.Succeeded)
            this.Message = "You don't have the necessary permissions for this action";

        return result.Succeeded;
    }

    protected async ValueTask<bool> CheckRecordAuthorization(TRecord record ,string policyName)
    {
        var id = Guid.Empty;
        if (record is IAuthRecord rec)
            id = rec.OwnerId;

        var authstate = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
        var result = await this.AuthorizationService.AuthorizeAsync(authstate.User, id, policyName);

        if (!result.Succeeded)
            this.Message = "You don't have the necessary permissions oin the object for this action";

        return result.Succeeded;
    }
}
```
## List Services

The purpose of the List Services is to provide an `IEnumerable` collection of `TRecord.`

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

    public void SetServices(IServiceProvider services);
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(int startRecord, int pageSize);
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request);
    public ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest request);
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(IListQuery<TRecord> query);
}
```

### Standard Implementation

This is the standard implementation.  There are several versions of `GetRecordsAsync` based on input which make it long.

```csharp
public class StandardListService<TRecord, TEntity>
    : BaseViewService<TRecord, TEntity>, IListService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    public int PageSize { get; protected set; }
    public int StartIndex { get; protected set; }
    public int ListCount { get; protected set; }
    public IEnumerable<TRecord>? Records { get; private set; }
    public bool IsPaging => (PageSize > 0);
    public bool HasList => this.Records is not null;

    public StandardListService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
        : base(dataBroker, notifier, authenticationState, authorizationService)
    { }

    public async ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest itemsRequest)
    {
        await this.GetRecordsAsync(new ListProviderRequest<TRecord>(itemsRequest));
        return new ItemsProviderResult<TRecord>(this.Records ?? new List<TRecord>(), this.ListCount);
    }

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request)
        => await this.GetRecordsAsync(new ListQuery<TRecord>(request));

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(IListQuery<TRecord> query)
    {
        this.Message = String.Empty;
        this.PageSize = query.PageSize;
        this.StartIndex = query.StartIndex;

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

