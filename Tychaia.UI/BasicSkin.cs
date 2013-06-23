//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia.UI
{
    public class BasicSkin : ISkin
    {
        private IBasicSkin m_BasicSkin;

        public BasicSkin(IBasicSkin skin)
        {
            this.m_BasicSkin = skin;
        }

        private void DrawRaised(XnaGraphics graphics, Rectangle layout)
        {
            graphics.FillRectangle(layout, this.m_BasicSkin.SurfaceColor);
            graphics.DrawLine(
                new Vector2(layout.X, layout.Y + layout.Height - 1),
                new Vector2(layout.X + layout.Width, layout.Y + layout.Height - 1),
                this.m_BasicSkin.DarkEdgeColor);
            graphics.DrawLine(
                new Vector2(layout.X + layout.Width - 1, layout.Y),
                new Vector2(layout.X + layout.Width - 1, layout.Y + layout.Height),
                this.m_BasicSkin.DarkEdgeColor);
            graphics.DrawLine(
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X + layout.Width, layout.Y),
                this.m_BasicSkin.LightEdgeColor);
            graphics.DrawLine(
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X, layout.Y + layout.Height),
                this.m_BasicSkin.LightEdgeColor);
        }

        private void DrawFlat(XnaGraphics graphics, Rectangle layout)
        {
            graphics.FillRectangle(layout, this.m_BasicSkin.SurfaceColor);
            graphics.DrawRectangle(layout, this.m_BasicSkin.LightEdgeColor);
        }

        private void DrawSunken(XnaGraphics graphics, Rectangle layout)
        {
            graphics.FillRectangle(layout, this.m_BasicSkin.SurfaceColor);
            graphics.DrawLine(
                new Vector2(layout.X, layout.Y + layout.Height - 1),
                new Vector2(layout.X + layout.Width, layout.Y + layout.Height - 1),
                this.m_BasicSkin.LightEdgeColor);
            graphics.DrawLine(
                new Vector2(layout.X + layout.Width - 1, layout.Y),
                new Vector2(layout.X + layout.Width - 1, layout.Y + layout.Height),
                this.m_BasicSkin.LightEdgeColor);
            graphics.DrawLine(
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X + layout.Width, layout.Y),
                this.m_BasicSkin.DarkEdgeColor);
            graphics.DrawLine(
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X, layout.Y + layout.Height),
                this.m_BasicSkin.DarkEdgeColor);
        }

        public void DrawButton(XnaGraphics graphics, Rectangle layout, Button button)
        {
            var offset = 0;
            if (button.State == ButtonState.Clicked)
            {
                this.DrawSunken(graphics, layout);
                offset = 1;
            }
            else
                this.DrawRaised(graphics, layout);
            graphics.DrawStringCentered(
                layout.Center.X + offset,
                layout.Center.Y + offset,
                button.Text,
                centerVertical: true);
        }

        public void DrawCanvas(XnaGraphics graphics, Rectangle layout, Canvas canvas)
        {
            graphics.FillRectangle(layout, this.m_BasicSkin.BackSurfaceColor);
        }

        public void DrawFixedContainer(XnaGraphics graphics, Rectangle layout, FixedContainer fixedContainer)
        {
        }

        public void DrawLabel(XnaGraphics graphics, Rectangle layout, Label label)
        {
            graphics.DrawStringCentered(
                layout.Center.X,
                layout.Center.Y,
                label.Text,
                centerVertical: true);
        }

        public void DrawLink(XnaGraphics graphics, Rectangle layout, Link link)
        {
            graphics.DrawStringCentered(
                layout.Center.X,
                layout.Center.Y,
                link.Text,
                centerVertical: true,
                color: Color.Blue);
        }

        public void DrawVerticalContainer(XnaGraphics graphics, Rectangle layout, VerticalContainer verticalContainer)
        {
        }
    }
}

