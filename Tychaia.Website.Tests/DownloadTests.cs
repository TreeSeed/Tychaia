using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Argotic.Syndication;
using Ninject;
using Ninject.MockingKernel.Moq;
using Tychaia.Globals;
using Tychaia.Website.Cachable;
using Tychaia.Website.Controllers;
using Tychaia.Website.ViewModels;
using Xunit;

namespace Tychaia.Website.Tests
{
    public class DownloadTests
    {
        [Fact]
        public void BuildStatusIsPassedThroughFromBuildServerClass()
        {
            var kernel = new MoqMockingKernel();
            kernel.Unbind<IBuildServer>();
            var mock = kernel.GetMock<IBuildServer>();

            mock.Setup(m => m.IsBuildServerOnline()).Returns(false);
            var controller = kernel.Get<DownloadController>();
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsType<DownloadViewModel>(viewResult.Model);
            Assert.Equal(false, ((DownloadViewModel)viewResult.Model).BuildServerOnline);

            mock.Setup(m => m.IsBuildServerOnline()).Returns(true);
            result = controller.Index();
            Assert.IsType<ViewResult>(result);
            viewResult = result as ViewResult;
            Assert.IsType<DownloadViewModel>(viewResult.Model);
            Assert.Equal(true, ((DownloadViewModel)viewResult.Model).BuildServerOnline);
        }

        private string GetIndexView()
        {
            return new FileInfo(
                Path.Combine(
                    Environment.CurrentDirectory,
                    "../../../Tychaia.Website/Views/Download/Index.cshtml")).FullName;
        }

        [Fact]
        public void IndexRendersWhenBuildServerIsOnline()
        {
            Assert.DoesNotThrow(() =>
                RazorHelper<DownloadViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new DownloadViewModel { BuildServerOnline = true }));
        }

        [Fact]
        public void IndexRendersWhenBuildServerIsOffline()
        {
            Assert.DoesNotThrow(() =>
                RazorHelper<DownloadViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new DownloadViewModel { BuildServerOnline = false }));
        }

        [Fact]
        public void IndexDisplaysLinksWhenBuildServerIsOnline()
        {
            var html = RazorHelper<DownloadViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new DownloadViewModel { BuildServerOnline = true });
            Assert.Contains("Download Pre-Alpha for Linux", html.Text);
            Assert.Contains("Download Pre-Alpha for Windows", html.Text);
        }

        [Fact]
        public void IndexDisplaysMessageWhenBuildServerIsOffline()
        {
            var html = RazorHelper<DownloadViewModel>.GenerateAndExecuteTemplate(
                this.GetIndexView(),
                new DownloadViewModel { BuildServerOnline = false });
            Assert.Contains("The build server is currently offline", html.Text);
        }
    }
}

