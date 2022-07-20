/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public interface IEntityUIService<TEntity>
    where TEntity : class, IEntity 
{
    public string Url { get;}

    public string SingleTitle { get;}

    public string PluralTitle { get; }

    public Type? EditForm { get; }

    public Type? ViewForm { get; }

    public string DefaultSortField { get; }

    public bool DefaultSortDescending { get; }

    public object? RecordAuthResource { get; }

    public Task AddRecordAsync(bool isModal, ModalOptions options);

}
