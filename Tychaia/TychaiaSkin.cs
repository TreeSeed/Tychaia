// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class TychaiaSkin : ISkin
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly BasicSkin m_BasicSkin;

        private readonly IAssetManager m_AssetManager;

        public TychaiaSkin(
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            BasicSkin basicSkin)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_BasicSkin = basicSkin;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
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

        public void DrawButton(IRenderContext context, Rectangle layout, Button button)
        {
            this.m_2DRenderUtilities.RenderRectangle(
                context,
                layout,
                Color.White,
                true);
            this.m_2DRenderUtilities.RenderText(
                context,
                new Vector2(
                    layout.Center.X,
                    layout.Center.Y),
                button.Text,
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }

        public void DrawCanvas(IRenderContext context, Rectangle layout, Canvas canvas)
        {
        }

        public void DrawFixedContainer(IRenderContext context, Rectangle layout, FixedContainer fixedContainer)
        {
        }

        public void DrawLabel(IRenderContext context, Rectangle layout, Label label)
        {
            this.m_BasicSkin.DrawLabel(context, layout, label);
        }

        public void DrawLink(IRenderContext context, Rectangle layout, Link link)
        {
            this.m_BasicSkin.DrawLink(context, layout, link);
        }

        public void DrawVerticalContainer(IRenderContext context, Rectangle layout, VerticalContainer verticalContainer)
        {
        }

        public void DrawHorizontalContainer(
            IRenderContext context,
            Rectangle layout,
            HorizontalContainer horizontalContainer)
        {
        }

        public void DrawMenuItem(IRenderContext context, Rectangle layout, MenuItem menuItem)
        {
            this.m_BasicSkin.DrawMenuItem(context, layout, menuItem);
        }

        public void DrawMenuList(IRenderContext context, Rectangle layout, MenuItem menuItem)
        {
            this.m_BasicSkin.DrawMenuList(context, layout, menuItem);
        }

        public void DrawMainMenu(IRenderContext context, Rectangle layout, MainMenu mainMenu)
        {
            this.m_BasicSkin.DrawMainMenu(context, layout, mainMenu);
        }

        public void DrawTreeView(IRenderContext context, Rectangle layout, TreeView treeView)
        {
            this.m_BasicSkin.DrawTreeView(context, layout, treeView);
        }

        public void DrawTreeItem(IRenderContext context, Rectangle layout, TreeItem treeItem)
        {
            this.m_BasicSkin.DrawTreeItem(context, layout, treeItem);
        }

        public void DrawSingleContainer(IRenderContext context, Rectangle layout, SingleContainer singleContainer)
        {
        }

        public void DrawTextBox(IRenderContext context, Rectangle layout, TextBox textBox)
        {
            this.m_BasicSkin.DrawTextBox(context, layout, textBox);
        }

        public void DrawForm(IRenderContext context, Rectangle layout, Form form)
        {
            this.m_BasicSkin.DrawForm(context, layout, form);
        }

        public void DrawFontViewer(IRenderContext context, Rectangle layout, FontViewer fontViewer)
        {
            this.m_BasicSkin.DrawFontViewer(context, layout, fontViewer);
        }

        public void DrawFileSelect(IRenderContext context, Rectangle layout, FileSelect fileSelect)
        {
            this.m_BasicSkin.DrawFileSelect(context, layout, fileSelect);
        }

        public void DrawTextureViewer(IRenderContext context, Rectangle layout, TextureViewer textureViewer)
        {
            this.m_BasicSkin.DrawTextureViewer(context, layout, textureViewer);
        }

        public void DrawAudioPlayer(IRenderContext context, Rectangle layout, AudioPlayer audioPlayer)
        {
            this.m_BasicSkin.DrawAudioPlayer(context, layout, audioPlayer);
        }

        public void DrawWindow(IRenderContext context, Rectangle layout, Window window)
        {
            this.m_BasicSkin.DrawWindow(context, layout, window);
        }

        public void DrawListView(IRenderContext context, Rectangle layout, ListView listView)
        {
            this.m_BasicSkin.DrawListView(context, layout, listView);
        }

        public void DrawListItem(IRenderContext context, Rectangle layout, ListItem listItem)
        {
            this.m_BasicSkin.DrawListItem(context, layout, listItem);
        }

        public Vector2 MeasureText(IRenderContext context, string text)
        {
            return this.m_2DRenderUtilities.MeasureText(
                context,
                text,
                this.m_AssetManager.Get<FontAsset>("font.Default"));
        }
    }
}
