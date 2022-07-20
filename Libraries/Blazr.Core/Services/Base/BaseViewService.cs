/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class BaseViewService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected ICQSDataBroker DataBroker;
    protected INotificationService<TEntity> Notifier;
    protected AuthenticationStateProvider AuthenticationStateProvider;
    protected IAuthorizationService AuthorizationService;

    public string? Message { get; protected set; }

    public BaseViewService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
    {
        this.DataBroker = dataBroker;
        this.Notifier = notifier;
        this.AuthenticationStateProvider = authenticationState;
        this.AuthorizationService = authorizationService;
    }

    public void SetServices(IServiceProvider services)
    {
        this.Notifier = services.GetService(typeof(INotificationService<TEntity>)) as INotificationService<TEntity> ?? default!;
        this.AuthenticationStateProvider = services.GetService(typeof(AuthenticationStateProvider)) as AuthenticationStateProvider ?? default!;
        this.AuthorizationService = services.GetService(typeof(IAuthorizationService)) as IAuthorizationService ?? default!;
    }

    protected async ValueTask<bool> CheckAuthorization(string policyName)
    {
        var authstate = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        var result = await this.AuthorizationService.AuthorizeAsync(authstate.User, null, policyName);

        if (!result.Succeeded)
            this.Message = "You don't have the necessary permissions for this action";

        return result.Succeeded;
    }

    protected async ValueTask<bool> CheckRecordAuthorization(TRecord record ,string policyName)
    {
        var id = Guid.Empty;
        if (record is IAuthRecord rec)
            id = rec.OwnerId;

        var authstate = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
        var result = await this.AuthorizationService.AuthorizeAsync(authstate.User, id, policyName);

        if (!result.Succeeded)
            this.Message = "You don't have the necessary permissions oin the object for this action";

        return result.Succeeded;
    }

}

