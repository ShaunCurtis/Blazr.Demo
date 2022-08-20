/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Controllers;

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
