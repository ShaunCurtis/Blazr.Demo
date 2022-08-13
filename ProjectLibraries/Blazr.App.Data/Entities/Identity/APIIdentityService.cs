/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Data;

public class APIIdentityService
    :IIdentityService
{
    private HttpClient _httpClient;

    public APIIdentityService(HttpClient httpClient)
        => _httpClient = httpClient;

    public async ValueTask<IdentityQueryResult> GetIdentity(Guid Uid)
    {
        IdentityQueryResult? result = null;

        var response = await _httpClient.PostAsJsonAsync<IdentityQuery>($"/api/identity/", new IdentityQuery { IdentityId = Uid });

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<IdentityQueryResult>();

        return result ?? new IdentityQueryResult();
    }
}
