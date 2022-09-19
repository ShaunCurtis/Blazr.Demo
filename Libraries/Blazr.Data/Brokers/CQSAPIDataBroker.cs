/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Net.Http.Json;

namespace Blazr.Data;

public sealed class CQSAPIDataBroker
    : ICQSDataBroker
{
    private HttpClient _httpClient;
    private IIdentityProvider? _identityProvider;
    public CQSAPIDataBroker(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public CQSAPIDataBroker(HttpClient httpClient, IIdentityProvider identityProvider)
    {
        _httpClient = httpClient;
        _identityProvider = identityProvider;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new()
    {
        ListProviderResult<TRecord>? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var request = APIListProviderRequest<TRecord>.GetRequest(query);

        this.SetHTTPClientSecurityHeader();
        var response = await _httpClient.PostAsJsonAsync<APIListProviderRequest<TRecord>>($"/api/{entityname}/listquery", request, query.CancellationToken);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result ?? ListProviderResult<TRecord>.Failure($"{response.StatusCode} = {response.ReasonPhrase}");
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new()
    {
        RecordProviderResult<TRecord>? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var request = APIRecordProviderRequest<TRecord>.GetRequest(query);

        this.SetHTTPClientSecurityHeader();
        var response = await _httpClient.PostAsJsonAsync<APIRecordProviderRequest<TRecord>>($"/api/{entityname}/recordquery", request, query.CancellationToken);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<RecordProviderResult<TRecord>>();

        return result ?? RecordProviderResult<TRecord>.Failure($"{response.StatusCode} = {response.ReasonPhrase}");
    }

    public async ValueTask<FKListProviderResult<TRecord>> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new()
    {
        FKListProviderResult<TRecord>? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var request = APIFKListQueryProviderRequest<TRecord>.GetRequest(query);

        this.SetHTTPClientSecurityHeader();
        var response = await _httpClient.PostAsJsonAsync<APIFKListQueryProviderRequest<TRecord>>($"/api/{entityname}/fklistquery", request, query.CancellationToken);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<FKListProviderResult<TRecord>>();

        return result ?? FKListProviderResult<TRecord>.Failure($"{response.StatusCode} = {response.ReasonPhrase}"); ;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var request = APICommandProviderRequest<TRecord>.GetRequest(command);

        this.SetHTTPClientSecurityHeader();
        var response = await _httpClient.PostAsJsonAsync<APICommandProviderRequest<TRecord>>($"/api/{entityname}/addrecordcommand", request, command.CancellationToken);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();

        return result ?? CommandResult.Failure($"{response.StatusCode} = {response.ReasonPhrase}"); ;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var request = APICommandProviderRequest<TRecord>.GetRequest(command);

        this.SetHTTPClientSecurityHeader();
        var response = await _httpClient.PostAsJsonAsync<APICommandProviderRequest<TRecord>>($"/api/{entityname}/updaterecordcommand", request, command.CancellationToken);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();

        return result ?? CommandResult.Failure($"{response.StatusCode} = {response.ReasonPhrase}"); ;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new()
    {
        CommandResult? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var request = APICommandProviderRequest<TRecord>.GetRequest(command);

        this.SetHTTPClientSecurityHeader();
        var response = await _httpClient.PostAsJsonAsync<APICommandProviderRequest<TRecord>>($"/api/{entityname}/deleterecordcommand", request, command.CancellationToken);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResult>();

        return result ?? CommandResult.Failure($"{response.StatusCode} = {response.ReasonPhrase}"); ;
    }

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();

    private void SetHTTPClientSecurityHeader()
    {
        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            _httpClient.DefaultRequestHeaders.Remove("Authorization");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization") && _identityProvider is not null)
            _httpClient.DefaultRequestHeaders.Add("Authorization", _identityProvider.GetHttpSecurityHeader());
    }
}
