/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherForecastEditContextValidator : AbstractValidator<WeatherForecastEditContext>
{
    public WeatherForecastEditContextValidator()
    {
        this.RuleFor(p => p.Summary)
            .MinimumLength(3)
            .WithState(p => p);

        this.RuleFor(p => p.Date)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Date must be in the future")
            .WithState(p => p);

        this.RuleFor(p => p.Temperature)
            .GreaterThanOrEqualTo(-60)
            .LessThanOrEqualTo(70)
            .WithState(p => p);
    }
}
