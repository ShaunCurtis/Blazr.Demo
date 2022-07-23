using System.Net.Http.Json;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Data;

public class CQSAPIDataBroker
    :ICQSDataBroker
{
    private HttpClient _httpClient;

    public CQSAPIDataBroker(HttpClient httpClient)
    { 
        _httpClient = httpClient;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordListQuery<TRecord> query) where TRecord : class, new()
    {
        ListProviderResult<TRecord> result = new ListProviderResult<TRecord>();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<RecordListQuery<TRecord>>($"/api/{entityname}/recordlistquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();
        
        return result;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IFilteredListQuery<TRecord> query) where TRecord : class, new()
    {
        //var queryType = query.GetType();
        //var handler = _serviceProvider.GetService<IFilteredListQueryHandler<TRecord>>();
        //if (handler == null)
        //    throw new NullReferenceException("No Handler service registed for the List Query");

        //var result = await handler.ExecuteAsync(query);
        //return result;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordGuidKeyQuery<TRecord> query) where TRecord : class, new()
    {
        RecordProviderResult<TRecord> result = new RecordProviderResult<TRecord>();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<RecordGuidKeyQuery<TRecord>>($"/api/{entityname}/recordguidkeyquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<RecordProviderResult<TRecord>>();

        return result;
    }

    public async ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new()
    {
        var handler = new FKListQueryHandler<TRecord, TDbContext>(_factory, query);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult result = new CommandResult();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<AddRecordCommand<TRecord>>($"/api/{entityname}/addrecordcommand", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<RecordProviderResult<TRecord>>();

        return result;

        var handler = new AddRecordCommandHandler<TRecord, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new UpdateRecordCommandHandler<TRecord, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new DeleteRecordCommandHandler<TRecord, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();
}
