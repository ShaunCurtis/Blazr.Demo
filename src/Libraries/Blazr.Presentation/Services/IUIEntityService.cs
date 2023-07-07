/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
 
namespace Blazr.Presentation;

public interface IUIEntityService<TEntityService>
    where TEntityService : IEntityService
{
    public string SingleDisplayName { get; }
    public string PluralDisplayName { get;}
    public Type? EditForm { get; }
    public Type? ViewForm { get; }
    public string Url { get; }
}
