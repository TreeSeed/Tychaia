// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Phabricator.Conduit;

namespace Tychaia.Network
{
    public static class TychaiaServerQuery
    {
        public static dynamic QueryServers()
        {
            var client = new ConduitClient("https://code.redpointsoftware.com.au/api")
            {
                User = "game-server", 
                Certificate =
                    "qyr2axzb2mwuc2vpq74zfebmjxitexas3ril4fhxr3lhq5ytg6p"
                    + "zt4abt6sxckxqjucszq5kijd3ju2pfuthfmerj6r37dokbwwmtk"
                    + "wrlldj3k6uklauf7pandjnkk6zutmohqpsxo3sbopj7wuurkzka"
                    + "42ewwds7zqyzje5ic4mt6upwgo4nj6bse2clfwe73xxhhnjmwpg"
                    + "zqxqgm6lyqv45evpbjmfsdw7cnz4jmhmkij75efjvzfma4eeuul"
            };

            return client.Do("serverlist.query", new { });
        }
    }
}