/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Blazr.Data;

public class CQSAPIDataBroker
    :ICQSDataBroker
{
    private HttpClient _httpClient;
    private ICQSAPIListHandlerFactory _CQSAPIListHandlerFactory;

    public CQSAPIDataBroker(HttpClient httpClient, ICQSAPIListHandlerFactory cQSAPIListHandlerFactory, Auth )
    { 
        _httpClient = httpClient;
        _CQSAPIListHandlerFactory = cQSAPIListHandlerFactory ;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new()
    {
        ListProviderResult<TRecord>? result = null;

        var entityname = (new TRecord()).GetType().Name;
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BlazrAuth", AuthHelper)
        var response = await _httpClient.PostAsJsonAsync<ListQuery<TRecord>>($"/api/{entityname}/listquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result ?? new ListProviderResult<TRecord>();
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IListQuery<TRecord> query) where TRecord : class, new()
        => await _CQSAPIListHandlerFactory.ExecuteAsync(query);

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new()
    {
        RecordProviderResult<TRecord>? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<RecordQuery<TRecord>>($"/api/{entityname}/recordquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<RecordProviderResult<TRecord>>();

        return result ?? new RecordProviderResult<TRecord>();
    }

    public async ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new()
    {
        FKListProviderResult? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<FKListQuery<TRecord>>($"/api/{entityname}/fklistquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<FKListProviderResult>();

        return result ?? new FKListProviderResult();
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<AddRecordCommand<TRecord>>($"/api/{entityname}/addrecordcommand", command);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();

        return result ?? new CommandResult();
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<UpdateRecordCommand<TRecord>>($"/api/{entityname}/updaterecordcommand", command);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();

        return result ?? new CommandResult();
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<DeleteRecordCommand<TRecord>>($"/api/{entityname}/deleterecordcommand", command);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();

        return result ?? new CommandResult();
    }

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();
}
