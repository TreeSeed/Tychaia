using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MakeMeAWorld
{
    public partial class Default : System.Web.UI.Page
    {
        public string HtmlLayerOptions;
        public string DefaultLayerOption;
        public bool ShowExperimentalOptions;

        protected void Page_Load(object sender, EventArgs e)
        {
            var experimentalValue = HttpContext.Current.Request.QueryString["experimental"];
            ShowExperimentalOptions = experimentalValue == "true" ||
                experimentalValue == "yes";

            HtmlLayerOptions = "";
            var list = BaseGenerator.GetListOfAvailableLayers(this.Context);
            var defaultLayer = BaseGenerator.GetDefaultAvailableLayer(this.Context);
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