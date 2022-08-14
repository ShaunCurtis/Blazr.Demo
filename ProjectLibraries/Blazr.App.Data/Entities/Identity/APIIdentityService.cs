
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
    
    public ClaimsPrincipal Identity { get; private set; } = new ClaimsPrincipal() ;

    public event EventHandler? IdentityChanged;

    public APIIdentityService(HttpClient httpClient)
        => _httpClient = httpClient;

    public async ValueTask<IdentityQueryResult> GetIdentityAsync(Guid Uid)
    {
        IdentityQueryResult result = new IdentityQueryResult();

        var response = await _httpClient.PostAsJsonAsync<IdentityQuery>($"/api/identity/", new IdentityQuery { IdentityId = Uid });

        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<IdentityQueryResult>() ?? new IdentityQueryResult();
            if (result.Success)
            {
                this.Identity = result.Identity;
                this.IdentityChanged?.Invoke(this, new EventArgs());
            }
        }

        return result;
    }
}
