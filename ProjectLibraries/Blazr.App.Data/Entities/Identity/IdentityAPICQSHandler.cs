/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

public class IdentityAPICQSHandler
    : IHandler<IdentityQuery, ValueTask<IdentityQueryResult>>, IIdentityCQSHandler
{
    private HttpClient _httpClient;

    public IdentityAPICQSHandler(HttpClient httpClient)
        => _httpClient = httpClient;

    public async ValueTask<IdentityQueryResult> ExecuteAsync(IdentityQuery query)
    {
        IdentityQueryResult result = new IdentityQueryResult();

        var response = await _httpClient.PostAsJsonAsync<IdentityQuery>($"/api/identity/authenticate", query);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<IdentityQueryResult>() ?? new IdentityQueryResult();

        return result;
    }
}
