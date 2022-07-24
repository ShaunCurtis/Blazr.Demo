/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

[ApiController]
public class DboWeatherLocationController : AppControllerBase<DboWeatherLocation>
{
    public DboWeatherLocationController(ICQSDataBroker dataBroker)
        : base(dataBroker)
    { }
}
