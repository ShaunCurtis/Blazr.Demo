using Blazr.App.Core;
using Blazr.Core;
using System.Net.Http;
using System.Security.Claims;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

public class APIIdentityQueryHandler<TDbContext>
    : IIdentityQueryHandler
        where TDbContext : DbContext
{
    private HttpClient _httpClient;

    public APIIdentityQueryHandler(HttpClient httpClient)
        => _httpClient = httpClient;

    public async ValueTask<IdentityRequestResult> ExecuteAsync(IdentityQuery query)
    {
        IdentityRequestResult? result = null;

        var request = APIIdentityProviderRequest.GetRequest(query);
        var response = await _httpClient.PostAsJsonAsync<APIIdentityProviderRequest>($"/api/identity/listquery", request, query.CancellationToken);

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<IdentityRequestResult>();

        return result ?? IdentityRequestResult.Failure($"{response.StatusCode} = {response.ReasonPhrase}");
    }
}
