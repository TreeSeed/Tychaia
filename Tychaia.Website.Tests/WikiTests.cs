using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Tychaia.Website.Models;
using Tychaia.Website.ViewModels;
using Xunit;

namespace Tychaia.Website.Tests
{
    public class WikiTests
    {
        private string GetIndexView()
        {
            return new FileInfo(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "../../../Tychaia.Website/Views/Wiki/Index.cshtml")).FullName;
        }

        private List<WikiPageModel> TestHierarchyScenario(
            IEnumerable<HierarchyScenarioChildType> grandchildren,
            List<WikiPageModel> defaultChildren = null
            )
        {
            if (defaultChildren == null)
                defaultChildren = new List<WikiPageModel>();
            var result = new List<WikiPageModel>();
            foreach (var grandchild in grandchildren)
            {
                switch (grandchild)
                {
                    case HierarchyScenarioChildType.Normal:
                        result.Add(new WikiPageModel
                        {
                            Title = "Normal",
                            Slug = "tychaia/normal",
                            Children = defaultChildren,
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.NullChildren:
                        result.Add(new WikiPageModel
                        {
                            Title = "Null Children",
                            Slug = "tychaia/normal",
                            Children = null,
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.NoTychaiaPrefix:
                        result.Add(new WikiPageModel
                        {
                            Title = "No tychaia/ prefix",
                            Slug = "normal",
                            Children = defaultChildren,
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.NullSlug:
                        result.Add(new WikiPageModel
                        {
                            Title = "Null Slug",
                            Slug = null,
                            Children = defaultChildren,
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.NullTitle:
                        result.Add(new WikiPageModel
                        {
                            Title = null,
                            Slug = "tychaia/nulltitle",
                            Children = defaultChildren,
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.WhitespaceSlug:
                        result.Add(new WikiPageModel
                        {
                            Title = "Whitespace Slug",
                            Slug = "   ",
                            Children = defaultChildren,
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.NoListTitle:
                        result.Add(new WikiPageModel
                        {
                            Title = "[NOLIST] Title",
                            Slug = "tychaia/hidden",
                            Children = new List<WikiPageModel>(),
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.AllNull:
                        result.Add(new WikiPageModel
                        {
                            Title = null,
                            Slug = null,
                            Children = null,
                            Content = new MvcHtmlString("Content")
                        });
                        break;
                    case HierarchyScenarioChildType.NullContent:
                        result.Add(new WikiPageModel
                        {
                            Title = "Normal",
                            Slug = "tychaia/normal",
                            Children = defaultChildren,
                            Content = null
                        });
                        break;
                    case HierarchyScenarioChildType.EmptyContent:
                        result.Add(new WikiPageModel
                        {
                            Title = "Normal",
                            Slug = "tychaia/normal",
                            Children = defaultChildren,
                            Content = new MvcHtmlString("")
                        });
                        break;
                    case HierarchyScenarioChildType.WhitespaceContent:
                        result.Add(new WikiPageModel
                        {
                            Title = "Normal",
                            Slug = "tychaia/normal",
                            Children = defaultChildren,
                            Content = new MvcHtmlString("   ")
                        });
                        break;
                }
            }
            return result;
        }

        #region Breadcrumb Testing

        [Fact]
        public void IndexDoesNotCrashWithNullBreadcrumbs()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel { Page = pages.First(), Breadcrumbs = null }));
        }

        [Fact]
        public void IndexDoesNotCrashWithEmptyBreadcrumbs()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>()
                }));
        }

        [Fact]
        public void IndexDoesNotCrashWithOneBreadcrumbNullTextNullSlug()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>
                    {
                        new BreadcrumbModel { Text = null, Slug = null }
                    }
                }));
        }

        [Fact]
        public void IndexDoesNotCrashWithOneBreadcrumbTextWithNullSlug()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>
                    {
                        new BreadcrumbModel { Text = "hello", Slug = null }
                    }
                }));
        }

        [Fact]
        public void IndexDoesNotCrashWithOneBreadcrumbTextWithSlug()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>
                    {
                        new BreadcrumbModel { Text = "hello", Slug = "hello" }
                    }
                }));
        }

        [Fact]
        public void IndexDoesNotCrashWithTwoBreadcrumbsNullTextNullSlug()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>
                    {
                        new BreadcrumbModel { Text = null, Slug = null },
                        new BreadcrumbModel { Text = null, Slug = null }
                    }
                }));
        }

        [Fact]
        public void IndexDoesNotCrashWithTwoBreadcrumbsTextWithNullSlug()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>
                    {
                        new BreadcrumbModel { Text = "hello", Slug = null },
                        new BreadcrumbModel { Text = "hello", Slug = null }
                    }
                }));
        }

        [Fact]
        public void IndexDoesNotCrashWithTwoBreadcrumbsTextWithSlug()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>
                    {
                        new BreadcrumbModel { Text = "hello", Slug = "hello" },
                        new BreadcrumbModel { Text = "hello", Slug = "hello" }
                    }
                }));
        }

        [Fact]
        public void IndexDoesNotCrashWithTwoBreadcrumbsNormal()
        {
            var pages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal
                });
            Assert.DoesNotThrow(() =>
                RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new WikiPageViewModel 
                { 
                    Page = pages.First(),
                    Breadcrumbs = new List<BreadcrumbModel>
                    {
                        new BreadcrumbModel { Text = "hello", Slug = "hello" },
                        new BreadcrumbModel { Text = "hello", Slug = null }
                    }
                }));
        }

        #endregion

        #region Coverage Tests

        [Fact]
        public void IndexDoesNotCrashInAnyScenario()
        {
            var grandchildren = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal,
                    HierarchyScenarioChildType.NullChildren,
                    HierarchyScenarioChildType.NoTychaiaPrefix,
                    HierarchyScenarioChildType.NullSlug,
                    HierarchyScenarioChildType.NullTitle,
                    HierarchyScenarioChildType.WhitespaceSlug,
                    HierarchyScenarioChildType.NoListTitle,
                    HierarchyScenarioChildType.AllNull,
                    HierarchyScenarioChildType.NullContent,
                    HierarchyScenarioChildType.EmptyContent,
                    HierarchyScenarioChildType.WhitespaceContent
                });
            var children = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                {
                    HierarchyScenarioChildType.Normal,
                    HierarchyScenarioChildType.NullChildren,
                    HierarchyScenarioChildType.NoTychaiaPrefix,
                    HierarchyScenarioChildType.NullSlug,
                    HierarchyScenarioChildType.NullTitle,
                    HierarchyScenarioChildType.WhitespaceSlug,
                    HierarchyScenarioChildType.NoListTitle,
                    HierarchyScenarioChildType.AllNull,
                    HierarchyScenarioChildType.NullContent,
                    HierarchyScenarioChildType.EmptyContent,
                    HierarchyScenarioChildType.WhitespaceContent
                }, grandchildren);
            var testPages = this.TestHierarchyScenario(
                new HierarchyScenarioChildType[]
                    {
                    HierarchyScenarioChildType.Normal,
                    HierarchyScenarioChildType.NullChildren,
                    HierarchyScenarioChildType.NoTychaiaPrefix,
                    HierarchyScenarioChildType.NullSlug,
                    HierarchyScenarioChildType.NullTitle,
                    HierarchyScenarioChildType.WhitespaceSlug,
                    HierarchyScenarioChildType.NoListTitle,
                    HierarchyScenarioChildType.AllNull,
                    HierarchyScenarioChildType.NullContent,
                    HierarchyScenarioChildType.EmptyContent,
                    HierarchyScenarioChildType.WhitespaceContent
                }, children);

            var breadcrumbsEmpty = new List<BreadcrumbModel>();
            var breadcrumbsOne = new List<BreadcrumbModel> { new BreadcrumbModel { Text = "hello" } };

            foreach (var page in testPages)
            {
                Assert.DoesNotThrow(() =>
                    RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                    this.GetIndexView(),
                    new WikiPageViewModel { Page = page, Breadcrumbs = breadcrumbsEmpty }));
                Assert.DoesNotThrow(() =>
                    RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                    this.GetIndexView(),
                    new WikiPageViewModel { Page = page, Breadcrumbs = breadcrumbsOne }));
                Assert.DoesNotThrow(() =>
                    RazorHelper<WikiPageViewModel>.GenerateAndExecuteTemplate(
                    this.GetIndexView(),
                    new WikiPageViewModel { Page = page, Breadcrumbs = null }));
            }
        }
        
        #endregion
    }
}

