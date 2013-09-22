// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public class DefaultViewportMode : IViewportMode
    {
        private ViewportMode m_ViewportMode = ViewportMode.Full;

        public int SidebarWidth { get; set; }

        public DefaultViewportMode()
        {
            this.SidebarWidth = 400;
        }

        public Viewport Get3DViewport(Viewport original)
        {
            switch (this.m_ViewportMode)
            {
                case ViewportMode.Full:
                    return new Viewport
                    {
                        X = original.X,
                        Y = original.Y,
                        Width = original.Width,
                        Height = original.Height,
                        MinDepth = original.MinDepth,
                        MaxDepth = original.MaxDepth
                    };
                case ViewportMode.Left:
                    return new Viewport
                    {
                        X = original.X,
                        Y = original.Y,
                        Width = original.Width - this.SidebarWidth,
                        Height = original.Height,
                        MinDepth = original.MinDepth,
                        MaxDepth = original.MaxDepth
                    };
                case ViewportMode.Right:
                    return new Viewport
                    {
                        X = original.X + this.SidebarWidth,
                        Y = original.Y,
                        Width = original.Width - this.SidebarWidth,
                        Height = original.Height,
                        MinDepth = original.MinDepth,
                        MaxDepth = original.MaxDepth
                    };
                case ViewportMode.Centre:
                    return new Viewport
                    {
                        X = original.X + this.SidebarWidth,
                        Y = original.Y,
                        Width = original.Width - this.SidebarWidth * 2,
                        Height = original.Height,
                        MinDepth = original.MinDepth,
                        MaxDepth = original.MaxDepth
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public void SetViewportMode(ViewportMode mode)
        {
            this.m_ViewportMode = mode;
        }
    }
}

