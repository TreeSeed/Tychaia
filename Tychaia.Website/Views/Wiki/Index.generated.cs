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

namespace Tychaia.Website.Views.Wiki
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
    
    #line 3 "..\..\Views\Wiki\Index.cshtml"
    using Cassette.Views;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Wiki\Index.cshtml"
    using Tychaia.Website;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Wiki/Index.cshtml")]
    public partial class Index : System.Web.Mvc.WebViewPage<Tychaia.Website.ViewModels.WikiPageViewModel>
    {
        public Index()
        {
        }
        public override void Execute()
        {




            
            #line 4 "..\..\Views\Wiki\Index.cshtml"
  
    ViewBag.Title = @Model.Page.Title;
    try
    {
        Bundles.Reference("Syntax");
    }
    catch (System.NullReferenceException)
    {
    }


            
            #line default
            #line hidden
WriteLiteral("\r\n<h2>\r\n");


            
            #line 16 "..\..\Views\Wiki\Index.cshtml"
     if (Model.Breadcrumbs != null)
    {
        foreach (var breadcrumb in Model.Breadcrumbs)
        {
            if (breadcrumb.Slug != null)
            {

            
            #line default
            #line hidden
WriteLiteral("                ");

WriteLiteral("<a href=\"/w/");


            
            #line 22 "..\..\Views\Wiki\Index.cshtml"
                         Write(breadcrumb.Slug);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 22 "..\..\Views\Wiki\Index.cshtml"
                                           Write(breadcrumb.Text);

            
            #line default
            #line hidden
WriteLiteral("</a> &gt;\r\n");


            
            #line 23 "..\..\Views\Wiki\Index.cshtml"
            }
            else
            {
                
            
            #line default
            #line hidden
            
            #line 26 "..\..\Views\Wiki\Index.cshtml"
           Write(breadcrumb.Text);

            
            #line default
            #line hidden
            
            #line 26 "..\..\Views\Wiki\Index.cshtml"
                                
            }
        }
    }

            
            #line default
            #line hidden
WriteLiteral("    <span class=\"actions\">\r\n    (\r\n        <a href=\"http://code.redpointsoftware." +
"com.au/phriction/edit/?slug=");


            
            #line 32 "..\..\Views\Wiki\Index.cshtml"
                                                                     Write(Model.Page.Slug);

            
            #line default
            #line hidden
WriteLiteral("\">Edit</a> &bull;\r\n        <a href=\"http://code.redpointsoftware.com.au/phriction" +
"/history/");


            
            #line 33 "..\..\Views\Wiki\Index.cshtml"
                                                                  Write(Model.Page.Slug);

            
            #line default
            #line hidden
WriteLiteral("\">History</a> &bull;\r\n        <a href=\"http://code.redpointsoftware.com.au/phrict" +
"ion/new/?slug=");


            
            #line 34 "..\..\Views\Wiki\Index.cshtml"
                                                                    Write(Model.Page.Slug);

            
            #line default
            #line hidden
WriteLiteral("\">Create Sub-Page</a>\r\n    )\r\n    </span>\r\n</h2>\r\n<div class=\"block\">\r\n    ");


            
            #line 39 "..\..\Views\Wiki\Index.cshtml"
Write(Model.Page.Content);

            
            #line default
            #line hidden
WriteLiteral("\r\n</div>\r\n");


            
            #line 41 "..\..\Views\Wiki\Index.cshtml"
 if (Model.Page.Children != null)
{

            
            #line default
            #line hidden
WriteLiteral("    <h2>Document Hierarchy</h2>\r\n");



WriteLiteral("    <div class=\"block\">\r\n        <ul>\r\n");


            
            #line 46 "..\..\Views\Wiki\Index.cshtml"
         foreach (var child in Model.Page.Children)
        {
        	if (child.Title == null || child.Title.StartsWith("[NOLIST]"))
        	{
        		continue;
    		}
        

            
            #line default
            #line hidden
WriteLiteral("            <li>\r\n");


            
            #line 54 "..\..\Views\Wiki\Index.cshtml"
             	if (child.Slug == null)
            	{
             		
            
            #line default
            #line hidden
            
            #line 56 "..\..\Views\Wiki\Index.cshtml"
          Write(child.Title);

            
            #line default
            #line hidden
            
            #line 56 "..\..\Views\Wiki\Index.cshtml"
                           
            	}
            	else
            	{

            
            #line default
            #line hidden
WriteLiteral("             \t\t<a href=\"/w/");


            
            #line 60 "..\..\Views\Wiki\Index.cshtml"
                      Write(child.Slug);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 60 "..\..\Views\Wiki\Index.cshtml"
                                   Write(child.Title);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n");


            
            #line 61 "..\..\Views\Wiki\Index.cshtml"
            	}

            
            #line default
            #line hidden

            
            #line 62 "..\..\Views\Wiki\Index.cshtml"
                 if (child.Children != null && child.Children.Count > 0)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <ul>\r\n");


            
            #line 65 "..\..\Views\Wiki\Index.cshtml"
                         foreach (var grandchild in child.Children)
                        {
				        	if (grandchild.Title == null || grandchild.Title.StartsWith("[NOLIST]"))
				        	{
				        		continue;
				    		}
				    		

            
            #line default
            #line hidden
WriteLiteral("                            <li>\r\n");


            
            #line 73 "..\..\Views\Wiki\Index.cshtml"
 				            	if (grandchild.Slug == null)
				            	{
				             		
            
            #line default
            #line hidden
            
            #line 75 "..\..\Views\Wiki\Index.cshtml"
              Write(grandchild.Title);

            
            #line default
            #line hidden
            
            #line 75 "..\..\Views\Wiki\Index.cshtml"
                                    
				            	}
				            	else
				            	{

            
            #line default
            #line hidden
WriteLiteral("\t\t\t\t             \t\t<a href=\"/w/");


            
            #line 79 "..\..\Views\Wiki\Index.cshtml"
                          Write(grandchild.Slug);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 79 "..\..\Views\Wiki\Index.cshtml"
                                            Write(grandchild.Title);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n");


            
            #line 80 "..\..\Views\Wiki\Index.cshtml"
				            	}

            
            #line default
            #line hidden
WriteLiteral("\t\t\t            \t</li>\r\n");


            
            #line 82 "..\..\Views\Wiki\Index.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </ul>\r\n");


            
            #line 84 "..\..\Views\Wiki\Index.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </li>\r\n");


            
            #line 86 "..\..\Views\Wiki\Index.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        </ul>\r\n    </div>\r\n");


            
            #line 89 "..\..\Views\Wiki\Index.cshtml"
}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591
