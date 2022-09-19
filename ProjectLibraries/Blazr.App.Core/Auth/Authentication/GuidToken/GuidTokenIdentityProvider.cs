using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazr.App.Core.Auth
{
    public class GuidTokenIdentityProvider : IIdentityProvider
    {

        public DboIdentity? Identity { get; set; }

        public string? GetHttpSecurityHeader()
        {
            return Identity is null
                ? null 
                : Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(this.Identity.Id.ToString()));

            //request.Headers.Add("Authorization", "Basic " + svcCredentials);
        }
    }
}
