/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class AppAuthenticationHandler : AuthenticationHandler<AppAuthOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string BasicSchemeName = "BlazrAuth";
    private ICQSDataBroker _dataBroker;

    public AppAuthenticationHandler(IOptionsMonitor<AppAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ICQSDataBroker dataBroker)
        : base(options, logger, encoder, clock)
    {
        _dataBroker = dataBroker;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        await Task.Yield();

        // Check the Headers and make sure we have a valid set
        if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            return AuthenticateResult.Fail("No Authorization Header detected");

        if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue? headerValue))
            return AuthenticateResult.Fail("No Authorization Header detected");

        if (!BasicSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.Fail("No Authorization Header detected");

        if (headerValue is null || headerValue.Parameter is null)
            return AuthenticateResult.Fail("No Token detected");

        // Get the User Guid from the security token
        var headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
        var uid = Encoding.UTF8.GetString(headerValueBytes);

        // Check we have a valid token and get the ClaimsPrincipal for the user
        if (!Guid.TryParse(uid, out Guid userId))
            return AuthenticateResult.Fail("Invalid Token submitted");

        var principal = await this.GetUserAsync(userId);

        if (principal is null)
            return AuthenticateResult.Fail("User does not Exist");

        // Create and return an AuthenticationTicket
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    public async Task<ClaimsPrincipal?> GetUserAsync(Guid Id)
    {
        // Get the user object from the database
        var query = new RecordGuidKeyQuery<DboUser>(Id);
        var result = await _dataBroker.ExecuteAsync(query);

        // Construct a ClaimsPrincipal object if the have a valid user
        if (result.Success && result.Record is not null && result.Record.Id.IsNotNull())
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, result.Record.Id.ToString()),
                    new Claim(ClaimTypes.Name, result.Record.Name),
                    new Claim(ClaimTypes.Role, result.Record.Role)
                }, BasicSchemeName));

        // No user so return null
        return null;
    }


}
