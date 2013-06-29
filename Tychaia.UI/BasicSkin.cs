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
            graphics.FillRectangle(layout, this.m_BasicSkin.DarkSurfaceColor);
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

        public void DrawHorizontalContainer(
            XnaGraphics graphics,
            Rectangle layout,
            HorizontalContainer horizontalContainer)
        {
        }

        public void DrawMenuItem(XnaGraphics graphics, Rectangle layout, MenuItem menuItem)
        {
            if (menuItem.Active)
                this.DrawRaised(graphics, layout);
            else
                this.DrawFlat(graphics, layout);
            graphics.DrawStringLeft(
                layout.X + 5,
                layout.Center.Y,
                menuItem.Text,
                centerVertical: true);
        }

        public void DrawMenuList(XnaGraphics graphics, Rectangle layout, MenuItem menuItem)
        {
            this.DrawRaised(graphics, layout);
        }

        public void DrawMainMenu(XnaGraphics graphics, Rectangle layout, MainMenu mainMenu)
        {
            this.DrawFlat(graphics, layout);
        }

        public void DrawTreeView(XnaGraphics graphics, Rectangle layout, TreeView treeView)
        {
            this.DrawSunken(graphics, layout);
        }

        public void DrawTreeItem(XnaGraphics graphics, Rectangle layout, TreeItem treeItem)
        {
            if (treeItem.Parent is TreeView)
            {
                var view = (treeItem.Parent as TreeView);
                if (view.SelectedItem == treeItem)
                {
                    this.DrawRaised(graphics, layout);
                }
            }
            graphics.DrawStringLeft(
                layout.X + 5,
                layout.Y,
                treeItem.Text);
        }

        public void DrawSingleContainer(XnaGraphics graphics, Rectangle layout, SingleContainer singleContainer)
        {
            this.DrawSunken(graphics, layout);
        }

        public int HeightForTreeItem
        {
            get { return 16; }
        }

        public int MainMenuHorizontalPadding
        {
            get { return 10; }
        }

        public int AdditionalMenuItemWidth
        {
            get { return 20; }
        }

        public int MenuItemHeight
        {
            get { return 24; }
        }
    }
}

