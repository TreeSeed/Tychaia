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
            IoC.ReplaceKernel(kernel);
            kernel.Unbind<IPhabricator>();
            var mock = kernel.GetMock<IPhabricator>();
            mock.Setup(m => m.GetTychaiaTuesdayIssue(It.IsAny<ConduitClient>(), 1)).Returns(issue);
            var controller = kernel.Get<TuesdayController>();
            var result = controller.Index(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsType<TychaiaTuesdayViewModel>(viewResult.Model);
            Assert.Equal(issue, ((TychaiaTuesdayViewModel)viewResult.Model).Issue);
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
                RazorHelper<TychaiaTuesdayViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new TychaiaTuesdayViewModel { Issue = issue }));
        }

        [Fact]
        public void IndexRendersWhenIssueDoesNotExist()
        {
            Assert.DoesNotThrow(() =>
                RazorHelper<TychaiaTuesdayViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new TychaiaTuesdayViewModel { Issue = null }));
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

            var html = RazorHelper<TychaiaTuesdayViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new TychaiaTuesdayViewModel { Issue = issue });
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

            var html = RazorHelper<TychaiaTuesdayViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new TychaiaTuesdayViewModel { Issue = issue });
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

            var html = RazorHelper<TychaiaTuesdayViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new TychaiaTuesdayViewModel { Issue = issue });
            Assert.Contains("Previous Issue", html.Text);
            Assert.Contains(issue.Title, html.Text);
            Assert.Contains(issue.Content, html.Text);
        }

        [Fact]
        public void IndexDisplaysMessageWhenIssueDoesNotExist()
        {
            var html = RazorHelper<TychaiaTuesdayViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new TychaiaTuesdayViewModel { Issue = null });
            Assert.Contains("Sorry, but this issue of Tychaia Tuesdays could not be found", html.Text);
        }
    }
}

