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
            var list = ImageGenerator.GetListOfAvailableLayers(this.Context);
            list = list.OrderBy(v => v.Substring(3)).ToList();
            foreach (var l in list)
            {
                if (l.Substring(3) == "Game World")
                    HtmlLayerOptions += "<option value=\"" + l + "\" selected=\"selected\">" + l.Substring(3) + "</option>";
                else
                    HtmlLayerOptions += "<option value=\"" + l + "\">" + l.Substring(3) + "</option>";
            }
        }
    }
}