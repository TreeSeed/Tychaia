﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tychaia.Website.Views.Blog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Cassette.Views;
    
    #line 2 "..\..\Views\Blog\Read.cshtml"
    using Tychaia.Website;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Blog/Read.cshtml")]
    public partial class Read : System.Web.Mvc.WebViewPage<Tychaia.Website.ViewModels.BlogReadViewModel>
    {
        public Read()
        {
        }
        public override void Execute()
        {



            
            #line 3 "..\..\Views\Blog\Read.cshtml"
  
    ViewBag.Title = "Tychaia Devlog";


            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 7 "..\..\Views\Blog\Read.cshtml"
 if (Model.Post == null) 
{

            
            #line default
            #line hidden
WriteLiteral("    <h2>\r\n        Post Not Found!\r\n        <span class=\"actions\">(<a href=\"/feed\"" +
">RSS Feed</a>)</span>\r\n    </h2>\r\n");



WriteLiteral("    <div class=\"block phame\">\r\n        <p>Sorry, but this blog post could not be " +
"found.</p>\r\n    </div>\r\n");


            
            #line 16 "..\..\Views\Blog\Read.cshtml"
}
else
{

            
            #line default
            #line hidden
WriteLiteral("    <h2>\r\n        ");


            
            #line 20 "..\..\Views\Blog\Read.cshtml"
   Write(Model.Post.Title);

            
            #line default
            #line hidden
WriteLiteral("\r\n        <span class=\"actions\">(\r\n");


            
            #line 22 "..\..\Views\Blog\Read.cshtml"
             if (Model.Post.Previous != null)
            {

            
            #line default
            #line hidden
WriteLiteral("                ");

WriteLiteral("\r\n                <a href=\"/blog/");


            
            #line 25 "..\..\Views\Blog\Read.cshtml"
                          Write(Model.Post.Previous);

            
            #line default
            #line hidden
WriteLiteral("\">Previous Post</a> &bull; \r\n                ");

WriteLiteral("\r\n");


            
            #line 27 "..\..\Views\Blog\Read.cshtml"
            }

            
            #line default
            #line hidden

            
            #line 28 "..\..\Views\Blog\Read.cshtml"
             if (Model.Post.Next != null)
            {

            
            #line default
            #line hidden
WriteLiteral("                ");

WriteLiteral("\r\n                <a href=\"/blog/");


            
            #line 31 "..\..\Views\Blog\Read.cshtml"
                          Write(Model.Post.Next);

            
            #line default
            #line hidden
WriteLiteral("\">Next Post</a> &bull; \r\n                ");

WriteLiteral("\r\n");


            
            #line 33 "..\..\Views\Blog\Read.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            <a href=\"/feed\">RSS Feed</a>\r\n        )</span>\r\n    </h2>\r\n");



WriteLiteral("    <div class=\"block phame\">\r\n        <div class=\"details\">\r\n            <div cl" +
"ass=\"author\">");


            
            #line 39 "..\..\Views\Blog\Read.cshtml"
                           Write(Model.Post.Author);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n            <div class=\"date\">");


            
            #line 40 "..\..\Views\Blog\Read.cshtml"
                         Write(DateTimeHelper.RenderTimestamp(Model.Post.UNIXDatePublished));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n        </div>\r\n        <span class=\"area\">\r\n            ");


            
            #line 43 "..\..\Views\Blog\Read.cshtml"
       Write(Html.Raw(Model.Post.Content));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </span>\r\n    </div>\r\n");



WriteLiteral(@"    <div class=""block"">
    
    <div id=""disqus_thread""></div>
    <script type=""text/javascript"">
        /* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */
        var disqus_shortname = 'tychaiatuesdays'; // required: replace example with your forum shortname

        /* * * DON'T EDIT BELOW THIS LINE * * */
        (function() {
            var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
            dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
            (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
        })();
    </script>
    <noscript>Please enable JavaScript to view the <a href=""http://disqus.com/?ref_noscript"">comments powered by Disqus.</a></noscript>
    <a href=""http://disqus.com"" class=""dsq-brlink"">comments powered by <span class=""logo-disqus"">Disqus</span></a>
    
    </div>
");


            
            #line 64 "..\..\Views\Blog\Read.cshtml"
}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591