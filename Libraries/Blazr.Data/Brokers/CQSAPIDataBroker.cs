/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;

namespace Blazr.Data;

public class CQSAPIDataBroker
    :ICQSDataBroker
{
    private HttpClient _httpClient;

    public CQSAPIDataBroker(HttpClient httpClient)
        => _httpClient = httpClient;

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new()
    {
        ListProviderResult<TRecord> result = new ListProviderResult<TRecord>();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<ListQuery<TRecord>>($"/api/{entityname}/listquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IListQuery<TRecord> query) where TRecord : class, new()
    {
        ListProviderResult<TRecord> result = new ListProviderResult<TRecord>();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<IListQuery<TRecord>>($"/api/{entityname}/ilistquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new()
    {
        RecordProviderResult<TRecord> result = new RecordProviderResult<TRecord>();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<RecordQuery<TRecord>>($"/api/{entityname}/recordquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<RecordProviderResult<TRecord>>();

        return result;
    }

    public async ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new()
    {
        FKListProviderResult result = new FKListProviderResult();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<FKListQuery<TRecord>>($"/api/{entityname}/fklistquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<FKListProviderResult>();

        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult result = new CommandResult();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<AddRecordCommand<TRecord>>($"/api/{entityname}/addrecordcommand", command);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();

        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult result = new CommandResult();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<UpdateRecordCommand<TRecord>>($"/api/{entityname}/updaterecordcommand", command);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();
        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult result = new CommandResult();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<DeleteRecordCommand<TRecord>>($"/api/{entityname}/deleterecordcommand", command);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();
        return result;
    }

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();
}
