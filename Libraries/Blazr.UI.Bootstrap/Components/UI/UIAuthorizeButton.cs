/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI.Bootstrap;

public class UIAuthorizeButton : UIButton
{
    [Parameter] public string Policy { get; set; } = String.Empty;

    [Parameter] public object? AuthFields { get; set; } = null;

    [CascadingParameter] public Task<AuthenticationState> AuthTask { get; set; } = default!;

    [Inject] protected IAuthorizationService authorizationService { get; set; } =default!;

    protected async override Task OnPreRenderAsync(bool firstRender)
    {
        if (AuthTask is null)
            throw new Exception($"{this.GetType().FullName} must have access to cascading Paramater {nameof(AuthTask)}");

        _show = await this.CheckPolicy();
    }

    protected virtual async Task<bool> CheckPolicy()
    {
        var state = await AuthTask!;
        var result = await this.authorizationService.AuthorizeAsync(state.User, AuthFields, Policy);
        return result.Succeeded;
    }
}
