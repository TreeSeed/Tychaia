// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using System.Web.Mvc;
using Ninject;
using Ninject.MockingKernel.Moq;
using Tychaia.Globals;
using Tychaia.Website.Cachable;
using Tychaia.Website.Controllers;
using Tychaia.Website.Models;
using Tychaia.Website.ViewModels;
using Xunit;
using Moq;
using Phabricator.Conduit;

namespace Tychaia.Website.Tests
{
    public class TychaiaTuesdayTests
    {
        #if FALSE
    
        [Fact]
        public void IssueIsPassedThroughFromPhabricator()
        {
            var issue = new TychaiaTuesdayIssueModel
            {
                Title = "Test Title",
                Content = "Test Content",
                Next = null,
                Previous = null
            };
            var kernel = new MoqMockingKernel();
            kernel.Unbind<IPhabricator>();
            var mock = kernel.GetMock<IPhabricator>();
            mock.Setup(m => m.GetTychaiaTuesdayIssue(It.IsAny<ConduitClient>(), 1)).Returns(issue);
            var controller = kernel.Get<BlogController>();
            var result = controller.Index(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsType<BlogIndexViewModel>(viewResult.Model);
            Assert.Equal(issue, ((BlogIndexViewModel)viewResult.Model).Issue);
        }

        private string GetIndexView()
        {
            return new FileInfo(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "../../../Tychaia.Website/Views/Tuesday/Index.cshtml")).FullName;
        }

        [Fact]
        public void IndexRendersWhenIssueExists()
        {
            var issue = new TychaiaTuesdayIssueModel
            {
                Title = "Test Title",
                Content = "Test Content"
            };

            Assert.DoesNotThrow(() =>
                RazorHelper<BlogIndexViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new BlogIndexViewModel { Issue = issue }));
        }

        [Fact]
        public void IndexRendersWhenIssueDoesNotExist()
        {
            Assert.DoesNotThrow(() =>
                RazorHelper<BlogIndexViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new BlogIndexViewModel { Issue = null }));
        }

        [Fact]
        public void IndexDisplaysIssueWhenIssueExists()
        {
            var issue = new TychaiaTuesdayIssueModel
            {
                Title = "Test Title",
                Content = "Test Content",
                Next = null,
                Previous = null
            };

            var html = RazorHelper<BlogIndexViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new BlogIndexViewModel { Issue = issue });
            Assert.Contains(issue.Title, html.Text);
            Assert.Contains(issue.Content, html.Text);
        }

        [Fact]
        public void IndexDisplaysNextLinkWhenNextIssueExists()
        {
            var issue = new TychaiaTuesdayIssueModel
            {
                Title = "Test Title",
                Content = "Test Content",
                Next = 2,
                Previous = null
            };

            var html = RazorHelper<BlogIndexViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new BlogIndexViewModel { Issue = issue });
            Assert.Contains("Next Issue", html.Text);
            Assert.Contains(issue.Title, html.Text);
            Assert.Contains(issue.Content, html.Text);
        }

        [Fact]
        public void IndexDisplaysPreviousLinkWhenPreviousIssueExists()
        {
            var issue = new TychaiaTuesdayIssueModel
            {
                Title = "Test Title",
                Content = "Test Content",
                Next = null,
                Previous = 2
            };

            var html = RazorHelper<BlogIndexViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new BlogIndexViewModel { Issue = issue });
            Assert.Contains("Previous Issue", html.Text);
            Assert.Contains(issue.Title, html.Text);
            Assert.Contains(issue.Content, html.Text);
        }

        [Fact]
        public void IndexDisplaysMessageWhenIssueDoesNotExist()
        {
            var html = RazorHelper<BlogIndexViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new BlogIndexViewModel { Issue = null });
            Assert.Contains("Sorry, but this issue of Tychaia Tuesdays could not be found", html.Text);
        }
        
        #endif
    }
}

