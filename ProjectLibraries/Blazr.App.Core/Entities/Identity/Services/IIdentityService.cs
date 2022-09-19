/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public interface IIdentityService
{
    public ClaimsPrincipal Identity { get; }   
    
    public ValueTask<IdentityRequestResult> GetIdentityAsync(Guid Uid);

    public event EventHandler? IdentityChanged;
}
