using System;
using Ninject;
using Ninject.MockingKernel.Moq;
using Tychaia.Globals;
using Tychaia.Website.Controllers;
using Xunit;
using Argotic.Syndication;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Linq;
using System.IO;
using System.Reflection;
using Tychaia.Website.ViewModels;

namespace Tychaia.Website.Tests
{
    public class HomeTests
    {
        [Fact]
        public void NullFeedIsPassedThroughFromPhabricator()
        {
            var kernel = new MoqMockingKernel();
            IoC.ReplaceKernel(kernel);
            kernel.Unbind<IPhabricator>();
            var mock = kernel.GetMock<IPhabricator>();
            mock.Setup(m => m.GetFeed("1")).Returns((AtomFeed)null);
            var controller = kernel.Get<HomeController>();
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsType<FeedViewModel>(viewResult.Model);
            Assert.Null(((FeedViewModel)viewResult.Model).Feed);
        }

        [Fact]
        public void FeedWithEntryIsPassedThroughFromPhabricator()
        {
            var entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("test entry"),
                DateTime.Now);
            var expectedFeed = new AtomFeed();
            expectedFeed.AddEntry(entry);

            var kernel = new MoqMockingKernel();
            IoC.ReplaceKernel(kernel);
            kernel.Unbind<IPhabricator>();
            var mock = kernel.GetMock<IPhabricator>();
            mock.Setup(m => m.GetFeed("1")).Returns(expectedFeed);
            var controller = kernel.Get<HomeController>();
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsType<FeedViewModel>(viewResult.Model);
            Assert.NotNull(((FeedViewModel)viewResult.Model).Feed);
            var feedObj = ((FeedViewModel)viewResult.Model).Feed;
            Assert.IsType<AtomFeed>(feedObj);
            var actualFeed = feedObj as AtomFeed;
            Assert.True(actualFeed.Entries.Count() == 1);
            Assert.True(actualFeed.Entries.First().Title.Content == "test entry");
            Assert.Equal(actualFeed, expectedFeed);
        }

        private string GetIndexView()
        {
            return new FileInfo(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "../../../Tychaia.Website/Views/Home/Index.cshtml")).FullName;
        }

        [Fact]
        public void IndexCrashesWithNoFeedTitle()
        {
            var feed = new AtomFeed();

            Assert.Throws<NullReferenceException>(() =>
                RazorHelper<FeedViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new FeedViewModel { Feed = feed }));
        }

        [Fact]
        public void IndexRendersWhenFeedHasTitleAndIsNotEmpty()
        {
            var feed = new AtomFeed();
            feed.Title = new AtomTextConstruct("My Feed");

            Assert.DoesNotThrow(() =>
                RazorHelper<FeedViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new FeedViewModel { Feed = feed }));
        }

        [Fact]
        public void IndexContainsTitleWhenFeedHasTitle()
        {
            var feed = new AtomFeed();
            feed.Title = new AtomTextConstruct("My Feed");

            var html = RazorHelper<FeedViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new FeedViewModel { Feed = feed });
            Assert.Contains("My Feed", html.Text);
        }

        [Fact]
        public void IndexCrashesWhenEntryHasNoTitle()
        {
            var feed = new AtomFeed();
            feed.Title = new AtomTextConstruct("My Feed");
            var entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("Entry Title"),
                DateTime.Now);
            feed.AddEntry(entry);

            Assert.Throws<NullReferenceException>(() =>
                RazorHelper<FeedViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new FeedViewModel { Feed = feed }));
        }

        [Fact]
        public void IndexRendersWhenFeedHasTitleAndEntryHasContent()
        {
            var feed = new AtomFeed();
            feed.Title = new AtomTextConstruct("My Feed");
            var entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("Entry Title"),
                DateTime.Now);
            entry.Content = new AtomContent("Entry Content");
            feed.AddEntry(entry);

            Assert.DoesNotThrow(() =>
                RazorHelper<FeedViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new FeedViewModel { Feed = feed }));
        }

        [Fact]
        public void IndexContainsContentWhenFeedHasTitleAndEntryHasContent()
        {
            var feed = new AtomFeed();
            feed.Title = new AtomTextConstruct("My Feed");
            var entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("Entry Title"),
                DateTime.Now);
            entry.Content = new AtomContent("Entry Content");
            feed.AddEntry(entry);

            var html = RazorHelper<FeedViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new FeedViewModel { Feed = feed });
            Assert.Contains("Entry Content", html.Text);
        }
    }
}

