/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Data;

public class AppCQSAPIDataBroker
    : CQSAPIDataBroker
{

    public AppCQSAPIDataBroker(HttpClient httpClient, ICQSAPIListHandlerFactory cQSAPIListHandlerFactory)
        :base(httpClient, cQSAPIListHandlerFactory)
    {}

    protected override void SetHTTPClientSecurityHeader()
    {
        //if (_authenticationIdentityService is not null)
        //    _httpClient.DefaultRequestHeaders.Authorization = _authenticationIdentityService.GetAPIAuthenticationHeader();
    }
}
