/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class GuidTokenAuthHandler
 : AuthenticationHandler<GuidTokenAuthOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string BasicSchemeName = "GuidTokenAuthentication";
    protected IIdentityQueryHandler _identityCQSHandler;

    public GuidTokenAuthHandler(IOptionsMonitor<GuidTokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IIdentityQueryHandler identityCQSHandler)
        : base(options, logger, encoder, clock)
        => _identityCQSHandler = identityCQSHandler;

    protected async override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
        await base.HandleChallengeAsync(properties);
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //Authorization header not in request
        if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            return AuthenticateResult.NoResult();

        //Invalid Authorization header
        var xyz = Request.Headers[AuthorizationHeaderName];
        if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue? headerValue))
            return AuthenticateResult.NoResult();

        //Not Basic authentication header
        if (!BasicSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        // No data to process
        if (headerValue is null || headerValue.Parameter is null)
            return AuthenticateResult.NoResult();

        //Decode the provided header data
        byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
        string guidString = Encoding.UTF8.GetString(headerValueBytes);

        if (!Guid.TryParse(guidString, out Guid value))
            return AuthenticateResult.Fail("Invalid Basic authentication header");

        IdentityQuery query = IdentityQuery.GetQuery(value);
        var result = await _identityCQSHandler.ExecuteAsync(query);

        if (!result.Success && result.Identity is not null)
            return AuthenticateResult.Fail("Invalid Uid");

        var identity = new ClaimsPrincipal(result.Identity!);

        var claims = new[] { new Claim(ClaimTypes.Name, BasicSchemeName)};
        identity.AddIdentity(new ClaimsIdentity(claims, Scheme.Name));

        //var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(identity, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}