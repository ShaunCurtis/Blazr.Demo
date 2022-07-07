/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class BaseEntityService
    :IEntityService
{
    public string Url { get; set; } = "record";

    public string Title { get; set; } = "Record" ;
}
