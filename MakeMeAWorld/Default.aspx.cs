using System;
using System.Collections.Generic;
using System.Linq;

namespace MakeMeAWorld
{
    public partial class Default : System.Web.UI.Page
    {
        public string HtmlLayerOptions;

        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlLayerOptions = "";
            var list = BaseGenerator.GetListOfAvailableLayers(this.Context);
            var defaultLayer = BaseGenerator.GetDefaultAvailableLayer(this.Context);
            foreach (var layer in list)
            {
                if (layer == defaultLayer)
                    HtmlLayerOptions += "<option value=\"" + layer + "\" selected=\"selected\">" + layer.Substring(3) + "</option>";
                else
                    HtmlLayerOptions += "<option value=\"" + layer + "\">" + layer.Substring(3) + "</option>";
            }
        }
    }
}