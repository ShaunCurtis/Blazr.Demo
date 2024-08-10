/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App.UI.Mud;

public static class WeatherForecastEditContextExtensions
{
    /// <summary>
    /// Provides specific state on Summry for the FluentUI autocomplete control
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool IsSummaryClean(this WeatherForecastEditContext context) => context.Summary is not null ? context.Summary.Equals(context.BaseRecord.Summary) : true;
}
