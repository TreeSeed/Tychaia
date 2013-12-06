// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Configuration;
using Phabricator.Conduit;

namespace Tychaia.Website
{
    public class ConduitClientProvider : IConduitClientProvider
    {
        public ConduitClient GetConduitClient()
        {
            var client = new ConduitClient("http://code.redpointsoftware.com.au/api");
            client.Certificate = ConfigurationManager.AppSettings["ConduitCertificate"];
            client.User = ConfigurationManager.AppSettings["ConduitUser"];
            return client;
        }
    }
}
