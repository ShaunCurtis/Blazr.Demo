/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

[ApiController]
public class DboUserController : AppControllerBase<DboUser>
{
    public DboUserController(ICQSDataBroker dataBroker)
        : base(dataBroker)
    { }
}
