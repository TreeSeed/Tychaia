// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace Tychaia.Website
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            bundles.Add<StylesheetBundle>("Content");
            bundles.Add<ScriptBundle>("Scripts");
            
            bundles.AddUrlWithAlias<StylesheetBundle>(
                "http://code.redpointsoftware.com.au/rsrc/css/core/syntax.css",
                "Syntax");
        }
    }
}
