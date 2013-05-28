using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Phabricator.Conduit;
using Tychaia.Website.Models;
using Tychaia.Website.ViewModels;

namespace Tychaia.Website.Controllers
{
    public class WikiController : Controller
    {
        private IPhabricator m_Phabricator;

        public WikiController(IPhabricator phabricator)
        {
            this.m_Phabricator = phabricator;
        }

        public ActionResult Index(string slug)
        {
            if (slug == null)
                slug = "";
            if (!string.IsNullOrWhiteSpace(slug))
                slug = "/" + slug;
            slug = "tychaia" + slug;
            var conduit = this.GetConduitClient();
            return View(new WikiPageViewModel
            {
                Page = GetContentFor(conduit, slug),
                Breadcrumbs = this.GenerateBreadcrumbs(conduit, slug)
            });
        }

        private ConduitClient GetConduitClient()
        {
            var client = new ConduitClient("http://code.redpointsoftware.com.au/api");
            client.Certificate = ConfigurationManager.AppSettings["ConduitCertificate"];
            client.User = ConfigurationManager.AppSettings["ConduitUser"];
            return client;
        }

        private WikiPageModel GetContentFor(ConduitClient client, string slug)
        {
            var page = this.m_Phabricator.GetWikiPage(client, slug);
            var remarkup = this.m_Phabricator.ProcessRemarkup(client, page.content);
            var hierarchy = this.m_Phabricator.GetWikiHierarchy(client, slug);
            return new WikiPageModel
            {
                Title = page.title,
                Slug = page.slug,
                Content = new MvcHtmlString(remarkup),
                Children = this.ConvertHierarchyResultToList(hierarchy)
            };
        }

        private List<WikiPageModel> ConvertHierarchyResultToList(dynamic hierarchyResult)
        {
            if (hierarchyResult == null)
                return null;
            var children = new List<WikiPageModel>();
            foreach (var child in hierarchyResult)
            {
                var childModel = new WikiPageModel();
                childModel.Title = child.Value.title;
                childModel.Slug = this.RewriteSlug(child.Value.slug);
                childModel.Children = child.Value.children.Count == 0 ? null :
                    new List<WikiPageModel>();
                foreach (var grandchild in child.Value.children)
                {
                    var grandchildModel = new WikiPageModel();
                    grandchildModel.Title = grandchild.Value.title;
                    grandchildModel.Slug = this.RewriteSlug(grandchild.Value.slug);
                    childModel.Children.Add(grandchildModel);
                }
                if (childModel.Children.Count == 0)
                    childModel.Children = null;
                children.Add(childModel);
            }
            if (children.Count == 0)
                return null;
            return children;
        }

        private string RewriteSlug(string slug)
        {
            if (slug.StartsWith("tychaia/", StringComparison.Ordinal))
                return slug.Substring("tychaia/".Length);
            return slug;
        }

        private List<BreadcrumbModel> GenerateBreadcrumbs(ConduitClient client, string slug)
        {
            dynamic page;
            var breadcrumbs = new List<BreadcrumbModel>();
            var slugBuilt = "";
            var parts = slug.Split('/');
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (slugBuilt.Length > 0)
                    slugBuilt += "/";
                slugBuilt += parts[i];
                page = this.m_Phabricator.GetWikiPage(client, slugBuilt);
                breadcrumbs.Add(new BreadcrumbModel
                {
                    Text = page.title,
                    Slug = this.RewriteSlug(page.slug)
                });
            }
            page = this.m_Phabricator.GetWikiPage(client, slug);
            breadcrumbs.Add(new BreadcrumbModel
            {
                Text = page.title,
                Slug = null
            });
            return breadcrumbs;
        }
    }
}
