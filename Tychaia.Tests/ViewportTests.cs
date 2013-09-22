// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Graphics;
using Ninject;
using Xunit;

namespace Tychaia.Tests
{
    public class ViewportTests
    {
        private IViewportMode GetViewportMode()
        {
            return new DefaultViewportMode();
        }

        [Fact]
        public void ValidateFullViewport()
        {
            var viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = 800,
                Height = 600
            };

            var viewportMode = this.GetViewportMode();
            viewportMode.SidebarWidth = 320;
            viewportMode.SetViewportMode(ViewportMode.Full);
            var newViewport = viewportMode.Get3DViewport(viewport);
            Assert.Equal(0, newViewport.X);
            Assert.Equal(0, newViewport.Y);
            Assert.Equal(800, newViewport.Width);
            Assert.Equal(600, newViewport.Height);
        }

        [Fact]
        public void ValidateLeftViewport()
        {
            var viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = 800,
                Height = 600
            };

            var viewportMode = this.GetViewportMode();
            viewportMode.SidebarWidth = 320;
            viewportMode.SetViewportMode(ViewportMode.Left);
            var newViewport = viewportMode.Get3DViewport(viewport);
            Assert.Equal(0, newViewport.X);
            Assert.Equal(0, newViewport.Y);
            Assert.Equal(800 - 320, newViewport.Width);
            Assert.Equal(600, newViewport.Height);
        }

        [Fact]
        public void ValidateRightViewport()
        {
            var viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = 800,
                Height = 600
            };

            var viewportMode = this.GetViewportMode();
            viewportMode.SidebarWidth = 320;
            viewportMode.SetViewportMode(ViewportMode.Right);
            var newViewport = viewportMode.Get3DViewport(viewport);
            Assert.Equal(320, newViewport.X);
            Assert.Equal(0, newViewport.Y);
            Assert.Equal(800 - 320, newViewport.Width);
            Assert.Equal(600, newViewport.Height);
        }

        [Fact]
        public void ValidateCentreViewport()
        {
            var viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = 800,
                Height = 600
            };

            var viewportMode = this.GetViewportMode();
            viewportMode.SidebarWidth = 320;
            viewportMode.SetViewportMode(ViewportMode.Centre);
            var newViewport = viewportMode.Get3DViewport(viewport);
            Assert.Equal(320, newViewport.X);
            Assert.Equal(0, newViewport.Y);
            Assert.Equal(800 - 320 * 2, newViewport.Width);
            Assert.Equal(600, newViewport.Height);
        }
    }
}

