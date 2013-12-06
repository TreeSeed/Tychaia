// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using Tychaia.Website.Models;

namespace Tychaia.Website.ViewModels
{
    public class BlogIndexViewModel
    {
        public IEnumerable<BlogPostModel> Posts { get; set; }
    }
}
