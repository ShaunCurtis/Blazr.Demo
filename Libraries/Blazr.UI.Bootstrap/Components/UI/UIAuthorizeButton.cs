/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI.Bootstrap;

public class UIAuthorizeButton : UIComponent
{
    [Parameter] public string Policy { get; set; } = String.Empty;

    [CascadingParameter] public Task<AuthenticationState>? AuthTask { get; set; }

    [Inject] protected IAuthorizationService? authorizationService { get; set; }

    public UIAuthorizeButton()
        => this.CssClasses.Add("btn");

    protected override void OnInitialized()
    {
        if (AuthTask is null)
            throw new Exception($"{this.GetType().FullName} must have access to cascading Paramater {nameof(AuthTask)}");
    }

    protected override string HtmlTag => "button";

    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this.Show && await this.CheckPolicy())
        {
            builder.OpenElement(0, this.HtmlTag);
            builder.AddAttribute(1, "class", this.CssClass);
            builder.AddMultipleAttributes(2, this.SplatterAttributes);

            if (!UserAttributes.ContainsKey("type"))
                builder.AddAttribute(3, "type", "button");

            if (Disabled)
                builder.AddAttribute(4, "disabled");

            if (ClickEvent.HasDelegate)
                builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ClickEvent));

            builder.AddContent(6, ChildContent);
            builder.CloseElement();
        }
    }

    protected virtual async Task<bool> CheckPolicy()
    {
        var state = await AuthTask!;
        var result = await this.authorizationService!.AuthorizeAsync(state.User, null, Policy);
        return result.Succeeded;
    }
}
