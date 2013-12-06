// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Web;

namespace Tychaia.Website.Models
{
    public class WikiPageModel
    {
        public string Title;
        public string Slug;
        public IHtmlString Content;
        public List<WikiPageModel> Children;
    }
}
