# Authentication and Authorization

For many, setting up and configuring authentication and authorization is a bit of a dark art.  You follow a set of instructions on some help page and hope it all comes together and works.

This article:

-  is an attempt to de-mystify the dark art.  
- help you understand what's actually going on when you add those configuration settings to `program`
- demonstrate how to customize the whole process.

## Demo Design

Users, referred to as Identities, log in using a Guid token.  There's no user name and password.  The database contains a single table:

```csharp
public record DboUser
{
    [Key] public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = String.Empty;
    public string Role { get; init; } = String.Empty;
}
```

There are three roles:

```csharp
    public const string AdminRole = "AdminRole";
    public const string UserRole = "UserRole";
    public const string VisitorRole = "VisitorRole";
```

Authorization should ensure Weather Forecasts can only be viewed by logging in users and edited by the record owner or an administrator.  `DboWeatherForecast` contains a new `OwnerId` field.

```csharp
public record DboWeatherForecast
    : IAuthRecord, IRecord
{
    [Key] public Guid Uid { get; init; }
    public Guid WeatherSummaryId { get; init; }
    public Guid WeatherLocationId { get; init; }
    public Guid OwnerId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; } = 0;
}
```

Along the way, we'll:

1. Build the infrastructure to Authenticate users based on holding a specific Uid.
2. Build custom server side authentication for API calls based on Uids.
3. Build custom policy based authorization policies and controls to control access down to the record level.

## Contexts

It's very easy to get confused about the context in which code is running and what context configuration.  So it's very important to understand:

 - the context in which code is running
 - the context a configuration applies to.

In general, we can divide an application into three contexts, each having an Authentication and an Authorization sub-context:

1. The **Http Request Context** - a web browser or other client making a request to a server.
2. The **Http Reponse Context** - the file or page provided back to the client.
3. The **SPA Context** - the application instance.

## Identity

`ClaimsPrincipal' is the primary DotNetCore class that represents an *Identity*.

The demo application defines an `IIdentityService` to manage the current authenticated *Identity*.  This:

1. Maintains a `ClaimsPrincipal' that represents the current *Identity*.
2. Provides a method to authenticate a Uid and obtain an *Identity*.
3. Provides an event that is raised when the *Identity* is changed.

## Identity Data Pipeline

We need to define a data pipleline for our `IdentityService`.  I'm using the CQS pattern, so first step is to define our result.

This looks like this:

```csharp
public record IdentityQueryResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public ClaimsPrincipal Identity { get; init; } = new ClaimsPrincipal();
}
```

And the request.  `TResult` is fixed as a `ValueTask` returning an `IdentityQueryResult`.  

```csharp
public class IdentityQuery
    : ICQSRequest<ValueTask<IdentityQueryResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
}
```

And a Handler interface - we will need both Server and API implementations.

```csharp
public interface IIdentityCQSHandler
{
    public ValueTask<IdentityQueryResult> ExecuteAsync(IdentityQuery query);
}
```

The server implementation uses the `DBContextFactory` to get a context and, executes a query against the `DboUser` recordset and builds a `ClaimsPrincipal`. 


```csharp
public class IdentityCQSHandler<TDbContext>
    : ICQSHandler<IdentityQuery, ValueTask<IdentityQueryResult>>, IIdentityCQSHandler
        where TDbContext : DbContext
{
    private IDbContextFactory<TDbContext> _factory;

    public IdentityCQSHandler(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<IdentityQueryResult> ExecuteAsync(IdentityQuery query)
    {
        if (query is not null)
        {
            using var dbContext = _factory.CreateDbContext();
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var record = await dbContext.Set<DboUser>().SingleOrDefaultAsync(item => item.Id == query.IdentityId);

            if (record is not null)
            {
                var identity = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, record.Id.ToString()),
                    new Claim(ClaimTypes.Name, record.Name),
                    new Claim(ClaimTypes.Role, record.Role)
                }, "Test authentication type"));

                return new IdentityQueryResult { Identity = identity, Success = true };
            }
        }

        return new IdentityQueryResult {Success = false, Message = "No query defined" };
    }
}
```

The WASM version simply calls the server API controller.

```csharp
public class IdentityAPICQSHandler
    : ICQSHandler<IdentityQuery, ValueTask<IdentityQueryResult>>, IIdentityCQSHandler
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
```

The API controller which plugs into the server registered handler.

```csharp
[ApiController]
public class IdentityController
    : ControllerBase
{
    protected IIdentityCQSHandler  _identityCQSHandler;

    public IdentityController(IIdentityCQSHandler identityCQSHandler)
        => _identityCQSHandler = identityCQSHandler;

    [Mvc.HttpPost]
    [Mvc.Route("/api/[controller]/authenicate")]
    public async Task<IdentityQueryResult> GetIdentity([FromBody] IdentityQuery query)
        => await _identityCQSHandler.ExecuteAsync(query);
}
```

## IdentityService

`IdentityService` holds the information about the currently logged in Identity.

First an interface:

```csharp
public interface IIdentityService
{
    public ClaimsPrincipal Identity { get; }   
    public ValueTask<IdentityQueryResult> GetIdentityAsync(Guid Uid);
    public event EventHandler? IdentityChanged;
}
```

And the implementation:

```csharp
public class IdentityService
    : IIdentityService
{
    private IIdentityCQSHandler _identityCQSHandler;

    public ClaimsPrincipal Identity { get; private set; } = new ClaimsPrincipal();

    public event EventHandler? IdentityChanged;

    public IdentityService(IIdentityCQSHandler identityCQSHandler)
        => _identityCQSHandler = identityCQSHandler;

    public async ValueTask<IdentityQueryResult> GetIdentityAsync(Guid Uid)
    {
        var result = await _identityCQSHandler.ExecuteAsync(new IdentityQuery { IdentityId = Uid });
        if (result.Success)
        {
            this.Identity = result.Identity;
            IdentityChanged?.Invoke(this, EventArgs.Empty);
        }
        return result;
    }
}
```

### Http Request Authentication

Whether you are running Server or hosted WASM, the entry point is the web server hosting the application.

In `Program` you will see:

```csharp
// Services section
Services.AddAuthentication();
Services.AddAuthorization();

// Http Request Pipeline
app.UseAuthentication();
app.UseAuthorization();
```

The services section adds the necessary service classes to the DI services.

`app.UseAuthentication()` adds a pipeline handler to actually perform authentication based on the information, normally a username/password pair, provided in the security header of the request.

### AppAuthenticationHandler

`AppAuthenticationHandler` is our custom handler shortly to "authenticate" on the provided Uid.

First we need to define some `AuthenticationSchemeOptions`.
```
public class AppAuthOptions : AuthenticationSchemeOptions
{
    public string Realm = "BlazrAuth";
}
```

And then the handler itself:

```csharp
public class AppAuthenticationHandler : AuthenticationHandler<AppAuthOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string BasicSchemeName = "BlazrAuth";
    private IIdentityService _identityService;

    public AppAuthenticationHandler(IOptionsMonitor<AppAuthOptions> options, IIdentityService identityService, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _identityService = identityService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // code
    }

    public async Task<ClaimsPrincipal?> GetUserAsync(Guid Id)
    {
        // Get the user object from the database
        var result = await _identityService.GetIdentityAsync(Id);
        
        // Construct a ClaimsPrincipal object if the have a valid user
        if (result.Success && result.Identity is not null)
            return result.Identity;

        // No user so return null
        return null;
    }
}
```

```csharp
protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
{
    await Task.Yield();

    // A set of checks to validate the supplied Security Header and it's contents - see thr repo code for the full set

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
```

The Authentication HttpHandler calls `HandleAuthenticateAsync` on the registered `AuthenticationHandler` and passes the provided information into the registered `AuthenticatonService`

The services registration for this new authentication looks like this:

```csharp
services.AddAuthentication("BlazrAuth").AddScheme<AppAuthOptions, AppAuthenticationHandler>("BlazrAuth", null);
```



### SPA Authentication

The SPA instance provides authentication through an instance of `AuthenticationStateProvider` provided by DI.  The out-of-the-box provider populates the `AuthenticationState`, which includes the `ClaimsPrincipal`, from the security header provided by the server.

In the application we build a custom provider that interfaces directly with an authentication provider on the server.

Our provider inherits from `AuthenticationStateProvider` and overrides `GetAuthenticationStateAsync`.  This uses the registered `IIdentity` service to get the `Identity` based on the provided Id.

```csharp
public class AppAuthenticationStateProvider : AuthenticationStateProvider
{
    private IIdentityService _identityService;
    private Guid _userId = Guid.Empty;

    public AppAuthenticationStateProvider(IIdentityService identityService)
        => _identityService = identityService;

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await _identityService.GetIdentityAsync(_userId);
        return new AuthenticationState(result.Identity);
    }

    public Task<AuthenticationState> ChangeUser(Guid userId)
    {
        _userId = userId;
        var task = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(task);
        return task;
    }
}
```

### Server Side Authentication

Server side authentication is provided through a custom `AuthenticationHandler`.

```csharp
public class AppAuthenticationHandler : AuthenticationHandler<AppAuthOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string BasicSchemeName = "BlazrAuth";
    private IIdentityService _identityService;

    public AppAuthenticationHandler(IOptionsMonitor<AppAuthOptions> options, IIdentityService identityService, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _identityService = identityService;
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
        var result = await _identityService.GetIdentityAsync(Id);
        
        // Construct a ClaimsPrincipal object if the have a valid user
        if (result.Success && result.Identity is not null)
            return result.Identity;

        // No user so return null
        return null;
    }
}
```

### Login Button
```csharp
@implements IDisposable
@namespace Blazr.App.UI

<UILoader State=this.state>

    <span class="mr-2 me-2">Change User:</span>
    <div class="w-25">
        <select id="userselect" class="form-control" @onchange="ChangeUser">
            @foreach (var value in this.Users)
            {
                @if (value.Key == _currentUserId)
                {
                    <option value="@value.Key" selected>@value.Value</option>
                }
                else
                {
                    <option value="@value.Key">@value.Value</option>
                }
            }
        </select>
    </div>
    <span class="text-nowrap ml-3 ms-3">
        <AuthorizeView>
            <Authorized>
                @if (this.user is not null && this.user.Identity is not null)
                {
                    <span>Hello, @this.user!.Identity!.Name</span>
                }
            </Authorized>
            <NotAuthorized>
                Not Logged In
            </NotAuthorized>
        </AuthorizeView>
    </span>
</UILoader>

@code {
    private ComponentState state = ComponentState.Loading;

    [CascadingParameter] public Task<AuthenticationState>? AuthTask { get; set; }

    [Inject] private AuthenticationStateProvider? AuthState { get; set; } = default!;

    private System.Security.Claims.ClaimsPrincipal user = new ClaimsPrincipal();
    private Guid _currentUserId = GuidExtensions.Null;

    protected async override Task OnInitializedAsync()
    {
        var authState = await AuthTask!;
        this.user = authState.User;
        this.state = ComponentState.Loaded;
        AuthState!.AuthenticationStateChanged += this.OnUserChanged;
    }

    private bool GetSelected(string value)
        => user.Identity!.Name!.Equals(value, StringComparison.CurrentCultureIgnoreCase);

    private async Task ChangeUser(ChangeEventArgs e)
    {
        if (AuthState is AppAuthenticationStateProvider && e.Value is not null && Guid.TryParse(e.Value.ToString(), out Guid Id))
            await ((AppAuthenticationStateProvider)AuthState).ChangeUser(Id);
    }

    private async void OnUserChanged(Task<AuthenticationState> state)
        => await this.GetUser(state);

    private async Task GetUser(Task<AuthenticationState> state)
    {
        var authState = await state;
        this.user = authState.User;
    }

    private Dictionary<Guid, string> Users = new Dictionary<Guid, string>
    {
        {GuidExtensions.Null, "Anonymous"},
        {new Guid("00000000-0000-0000-0000-000000000001"), "Visitor"},
        {new Guid("00000000-0000-0000-0000-100000000001"), "User-1"},
        {new Guid("00000000-0000-0000-0000-100000000002"), "User-2"},
        {new Guid("00000000-0000-0000-0000-100000000003"), "User-3"},
        {new Guid("00000000-0000-0000-0000-200000000001"), "Admin"},
    };

    public void Dispose()
        => AuthState!.AuthenticationStateChanged -= this.OnUserChanged;
}
```

### Authorize Button

```csharp
```