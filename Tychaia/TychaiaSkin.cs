//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class TychaiaSkin : ISkin
    {
        private IRenderUtilities m_RenderUtilities;
        private IAssetManager m_AssetManager;

        public TychaiaSkin(
            IRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
        }
        
        public void DrawButton(IRenderContext context, Rectangle layout, Button button)
        {
            this.m_RenderUtilities.RenderRectangle(
                context,
                layout,
                Color.White,
                filled: true);
            this.m_RenderUtilities.RenderText(
                context,
                new Vector2(
                    layout.Center.X,
                    layout.Center.Y),
                button.Text,
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                horizontalAlignment: HorizontalAlignment.Center,
                verticalAlignment: VerticalAlignment.Center);
        }

        public void DrawCanvas(IRenderContext context, Rectangle layout, Canvas canvas)
        {
        }

        public void DrawFixedContainer(IRenderContext context, Rectangle layout, FixedContainer fixedContainer)
        {
        }

        public void DrawLabel(IRenderContext context, Rectangle layout, Label label)
        {
            throw new NotSupportedException();
        }

        public void DrawLink(IRenderContext context, Rectangle layout, Link link)
        {
            throw new NotSupportedException();
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
            throw new NotSupportedException();
        }

        public void DrawMenuList(IRenderContext context, Rectangle layout, MenuItem menuItem)
        {
            throw new NotSupportedException();
        }

        public void DrawMainMenu(IRenderContext context, Rectangle layout, MainMenu mainMenu)
        {
            throw new NotSupportedException();
        }

        public void DrawTreeView(IRenderContext context, Rectangle layout, TreeView treeView)
        {
            throw new NotSupportedException();
        }

        public void DrawTreeItem(IRenderContext context, Rectangle layout, TreeItem treeItem)
        {
            throw new NotSupportedException();
        }

        public void DrawSingleContainer(IRenderContext context, Rectangle layout, SingleContainer singleContainer)
        {
            throw new NotSupportedException();
        }

        public void DrawTextBox(IRenderContext context, Rectangle layout, TextBox textBox)
        {
            throw new NotSupportedException();
        }
        
        public void DrawForm(IRenderContext context, Rectangle layout, Form form)
        {
            throw new NotSupportedException();
        }
        
        public void DrawFontViewer(IRenderContext context, Rectangle layout, FontViewer fontViewer)
        {
            throw new NotSupportedException();
        }

        public void DrawFileSelect(IRenderContext context, Rectangle layout, FileSelect fileSelect)
        {
            throw new NotSupportedException();
        }
        
        public void DrawTextureViewer(IRenderContext context, Rectangle layout, TextureViewer textureViewer)
        {
            throw new NotSupportedException();
        }
        
        public void DrawAudioPlayer(IRenderContext context, Rectangle layout, AudioPlayer audioPlayer)
        {
            throw new NotSupportedException();
        }

        public void DrawWindow(IRenderContext context, Rectangle layout, Window window)
        {
            throw new NotSupportedException();
        }
        
        public Vector2 MeasureText(IRenderContext context, string text)
        {
            return this.m_RenderUtilities.MeasureText(
                context,
                text,
                this.m_AssetManager.Get<FontAsset>("font.Default"));
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

