// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Web;

namespace MakeMeAWorld
{
    public class Default : System.Web.UI.Page
    {
        public string HtmlLayerOptions;
        public string DefaultLayerOption;
        public bool ShowExperimentalOptions;

        protected void Page_Load(object sender, EventArgs e)
        {
            var experimentalValue = HttpContext.Current.Request.QueryString["experimental"];
            ShowExperimentalOptions = true;

            HtmlLayerOptions = "";
            var generator = new JsonGenerator();
            var list = generator.GetListOfAvailableLayers(this.Context);
            var defaultLayer = generator.GetDefaultAvailableLayer(this.Context);
            foreach (var layer in list)
            {
                if (layer == defaultLayer)
                {
                    HtmlLayerOptions += "<option value=\"" + layer + "\" selected=\"selected\">" + layer.Substring(3) + "</option>";
                    DefaultLayerOption = layer;
                }
                else
                    HtmlLayerOptions += "<option value=\"" + layer + "\">" + layer.Substring(3) + "</option>";
            }
        }
    }
}
