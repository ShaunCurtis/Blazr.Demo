/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

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
        ListProviderResult<TRecord> result = new ListProviderResult<TRecord>();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<WeatherForecastListQuery>($"/api/{entityname}/ilistquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result;
    }

    private async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new()
    {
        ListProviderResult<TRecord> result = new ListProviderResult<TRecord>();

        var entityname = (new TRecord()).GetType().Name;
        var response = await _httpClient.PostAsJsonAsync<ListQuery<TRecord>>($"/api/{entityname}/listquery", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<ListProviderResult<TRecord>>();

        return result;
    }

}
