/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class SummaryOptions
{
    public static IEnumerable<string> Summaries => _summaries.AsEnumerable();  
    private static List<string> _summaries = new() {"Not Provided", "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
}
