/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class BaseEntityUIService<TEntity>
    : IEntityUIService<TEntity>
    where TEntity : class, IEntity
{
    private readonly NavigationManager _navigationManager;
    private readonly ModalService _modalService;

    public string Url { get; set; } = "record";

    public string SingleTitle { get; set; } = "record";

    public string PluralTitle { get; set; } = "records";

    public Type? EditForm { get; set; } = null;

    public Type? ViewForm { get; set; } = null;

    public string DefaultSortField { get; set; } = string.Empty;

    public bool DefaultSortDescending { get; set; } = false;

    public object? RecordAuthResource { get; set; } = null;

    public BaseEntityUIService(ModalService modalService, NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _modalService = modalService;
    }

    public async Task AddRecordAsync(bool useModalForms, ModalOptions options)
    {
        if (_modalService.IsModalFree && useModalForms && this.EditForm is not null)
        {
            await _modalService.Modal.ShowAsync(this.EditForm, options);
        }
        else
            _navigationManager!.NavigateTo($"/{this.Url}/edit/0");

    }


}
