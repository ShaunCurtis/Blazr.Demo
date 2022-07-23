/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

/// <summary>
/// A display only Input box for Active/Hidden fields
/// </summary>
public class InputReadOnlyActive : InputReadOnlyBoolean
{
    protected override string message => this.Value ? "Active" : "Hidden";
}
