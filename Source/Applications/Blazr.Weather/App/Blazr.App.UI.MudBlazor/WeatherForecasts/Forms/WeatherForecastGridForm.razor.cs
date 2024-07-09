using Blazr.App.Core;
using Blazr.EditStateTracker;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Mud = MudBlazor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazr.EditStateTracker.Components;

namespace Blazr.App.UI.MudBlazor;

public partial class WeatherForecastGridForm : ComponentBase
{
    private Mud.MudDataGrid<DmoWeatherForecast>? _grid;

    private async Task OnView(WeatherForecastId id)
    {
        var parameters = new Mud.DialogParameters() { { "WeatherForecastId", id } };
        var options = new Mud.DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        await DialogService.ShowAsync<WeatherForecastViewForm>("View Weather Forecast", parameters, options);
    }

    private async Task OnEdit(WeatherForecastId id)
    {
        var parameters = new Mud.DialogParameters() { { "WeatherForecastId", id } };
        var options = new Mud.DialogOptions { DisableBackdropClick = true, CloseOnEscapeKey = false, FullWidth = true };
        await DialogService.ShowAsync<WeatherForecastEditForm>("Edit Weather Forecast", parameters, options);

        //if (_grid is not null)
        //    await _grid.ReloadServerData();
    }
}
