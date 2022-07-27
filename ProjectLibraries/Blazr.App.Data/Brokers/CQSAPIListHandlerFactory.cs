/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

// ===========================
// Interfaces can't be serialized so we can't pass custom list queries through the IListQuery interface in web API calls.
// This factory matches custom ListQueries passed as IListQuery to the ICQSDataBroker to their matching concrete classes and 
// makes API calls using the concrete class.
// You need to add a `ExecuteAsync` handler and add a switch case to the public `ExecuteAsync` method 
// Note: there needs to be a corresponding Controller to receive the call.
// ===========================
public class CQSAPIListHandlerFactory : ICQSAPIListHandlerFactory
{
    private HttpClient _httpClient;
    
    public CQSAPIListHandlerFactory(HttpClient httpClient)
        => _httpClient = httpClient;

    public ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IListQuery<TRecord> query) where TRecord : class, new()
    {
        switch (query) 
        {
            case WeatherForecastListQuery item:
                return ExecuteAsync<TRecord>(item);

            case ListQuery<TRecord> item:
                return ExecuteAsync<TRecord>(item);

            default:
                throw new ArgumentException("No case defined for the ListQuery in CQSAPIListHandlerFactory");
        } 
    }

    private async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(WeatherForecastListQuery query) where TRecord : class, new()
    {
        ListProviderResult<TRecord>? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<WeatherForecastListQuery>($"/api/{entityname}/ilistquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result ?? new ListProviderResult<TRecord>();
    }

    private async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new()
    {
        ListProviderResult<TRecord>? result = null;

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<ListQuery<TRecord>>($"/api/{entityname}/listquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result ?? new ListProviderResult<TRecord>();
    }
}
