/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public interface IIdentityCQSHandler
{
    public ValueTask<IdentityQueryResult> ExecuteAsync(IdentityQuery query);
}
