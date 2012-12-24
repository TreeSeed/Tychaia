using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MakeMeAWorld
{
    public partial class Default : System.Web.UI.Page
    {
        public string m_HtmlLayerOptions;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_HtmlLayerOptions = "";
            var list = ImageGenerator.GetListOfAvailableLayers(this.Context);
            list = list.OrderBy(v => v.Substring(3)).ToList();
            foreach (var l in list)
            {
                if (l.Substring(3) == "Game World")
                    m_HtmlLayerOptions += "<option value=\"" + l + "\" selected=\"selected\">" + l.Substring(3) + "</option>";
                else
                    m_HtmlLayerOptions += "<option value=\"" + l + "\">" + l.Substring(3) + "</option>";
            }
        }
    }
}