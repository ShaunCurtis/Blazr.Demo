/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrViewerForm<TRecord, TEntity>
    : BlazrForm<IReadService<TRecord, TEntity>, TEntity>, IDisposable, IHandleEvent, IHandleAfterRender
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected virtual Type? EditControl => this.EntityUIService.EditForm;

    /// <summary>
    /// Overloaded component SetParametersAsync
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        // Applies the component parameter changes
        parameters.SetParameterProperties(this);

        if (isNew)
        {
            // Get the Title for the form
            if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
                this.FormTitle = $"{this.EntityUIService.SingleTitle} Viewer";

            // We're using Owning so creating our own services container for our Read Service
            // This service needs access to the SPA's Scoped services.  So calling this method on the service
            // sets those services to the correct SPA level instances of those objects
            this.Service.SetServices(SPAServiceProvider);

            // Loads the record
            await PreLoadRecordAsync();
            await this.LoadRecordAsync();

            // Sets up the evnt handler for record changes
            this.NotificationService.RecordChanged += OnChange;
        }

        // Calls the base version.
        await base.SetParametersAsync(ParameterView.Empty);

        // We aren't new any more
        isNew = false;
    }

    /// <summary>
    /// Method to load the Record
    /// </summary>
    /// <param name="render"></param>
    /// <returns></returns>
    private async Task LoadRecordAsync(bool render = false)
    {
        this.LoadState = ComponentState.Loading;

        var result = await this.Service.LoadRecordAsync(Id);

        this.LoadState = result
            ? ComponentState.Loaded
            : this.LoadState = ComponentState.UnAuthorized;

        if (render)
            this.InvokeStateHasChanged();
    }

    /// <summary>
    /// Event handler for a record changed notificatiion
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnChange(object? sender, RecordEventArgs e)
    {
        if (this.IsThisRecord(e.RecordId))
            await LoadRecordAsync(true);
    }

    /// <summary>
    /// Checks if we are requested the existing loaded record
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    protected virtual bool IsThisRecord(Guid Id)
    {
        if (this.Service.Record is IRecord)
            return ((IRecord)this.Service.Record).Uid == Id;

        return true;
    }

    /// <summary>
    /// Method to switch to the Editor for the record
    /// </summary>
    /// <returns></returns>
    protected virtual async Task EditRecordAsync()
    {
        // There are three possible options:
        //     1. We aren't in a modal but there's a modal available and we have an editform defined
        //     2. We are in a modal and want to switch the control in the modal to the edit form
        //     3. We don't have all the options available to use a modal so navigate to the edit form 
        //         (We don't know if one exists, but that's not our problem!)
        if (this.Modal is not null && this.ModalService.IsModalFree && this.EditControl is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", this.Id);
            options = this.GetEditOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EditControl, options);
        }
        else if (this.Modal is not null && this.EditControl is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", this.Id);
            options = this.GetEditOptions(options);
            await this.ModalService.Modal.SwitchAsync(this.EditControl, options);
        }
        // this needs testing in normal form for new component base
        else
            this.NavManager!.NavigateTo($"/{this.EntityUIService.Url}/edit/{Id}");
    }

    /// <summary>
    /// Method to let us add some extra options to the ModalOptions before we open the modal dialog
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ModalOptions GetEditOptions(ModalOptions? options)
        => options ?? new ModalOptions();

    public void Dispose()
        => this.NotificationService.RecordChanged -= OnChange;
}
