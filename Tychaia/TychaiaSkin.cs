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
    public class TychaiaSkin : ISkin, ITychaiaSkin
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
        
        public int ListVerticalPadding
        {
            get { return this.UIBorderSize; }
        } 
       
        public int ListHorizontalPadding
        {
            get { return this.UIBorderSize; }
        } 
            
        public int UIBorderSize
        {
            get { return 10; }
        }
        
        // UI Elements
        // This if for major elements such as windows
        public void DrawUIBorder(IRenderContext context, Rectangle layout, Side side)
        {
            // TODO: Funky maths that selects textures based off of a random selection and blends
            switch (side)
            {
                case Side.Top:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X, layout.Y),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.borders.1"),
                        new Vector2(layout.Width, this.UIBorderSize));
                    break;
                case Side.Bottom:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X, layout.Y + layout.Height - this.UIBorderSize),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.borders.1"),
                        new Vector2(layout.Width, this.UIBorderSize));
                    break;
                case Side.Left:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X + this.UIBorderSize, layout.Y),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.borders.1"),
                        new Vector2(layout.Height, this.UIBorderSize), 
                        rotation: (float)(Math.PI / 2));
                    break;
                case Side.Right:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X + layout.Width, layout.Y),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.borders.1"),
                        new Vector2(layout.Height, this.UIBorderSize),
                        rotation: (float)(Math.PI / 2));
                    break;
            }
        }
        
        public void DrawUICorner(IRenderContext context, Rectangle layout, Corner corner, string button = null)
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X - (float)(this.UIBorderSize * 0.3), layout.Y - (float)(this.UIBorderSize * 0.3)),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.corners.top_left.Default"),
                        new Vector2((float)(this.UIBorderSize * 2.2), (float)(this.UIBorderSize * 2.2)));
                    break;
                case Corner.TopRight:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X + layout.Width - ((float)(this.UIBorderSize * 2.2)) + (float)(this.UIBorderSize * 0.3), layout.Y - (float)(this.UIBorderSize * 0.3)),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.corners.top_right.Default"),
                        new Vector2((float)(this.UIBorderSize * 2.2), (float)(this.UIBorderSize * 2.2)));
                    break;
                case Corner.BottomLeft:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X - (float)(this.UIBorderSize * 0.3), layout.Y + layout.Height - ((float)(this.UIBorderSize * 2.2)) + (float)(this.UIBorderSize * 0.3)),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.corners.bottom_left.Default"),
                        new Vector2((float)(this.UIBorderSize * 2.2), (float)(this.UIBorderSize * 2.2)));
                    break;
                case Corner.BottomRight:
                    this.m_2DRenderUtilities.RenderTexture(
                        context,
                        new Vector2(layout.X + layout.Width - ((float)(this.UIBorderSize * 2.2)) + (float)(this.UIBorderSize * 0.3), layout.Y + layout.Height - ((float)(this.UIBorderSize * 2.2)) + (float)(this.UIBorderSize * 0.3)),
                        this.m_AssetManager.Get<TextureAsset>("texture.ui.corners.bottom_right.Default"),
                        new Vector2((float)(this.UIBorderSize * 2.2), (float)(this.UIBorderSize * 2.2)));
                    break;
                default:
                    throw new NotSupportedException("DrawUICorner: Corner not specified.");
            }
        }
        
        // Gets the texture and tiles it. Crops it off on the edges.
        public void DrawUIBackground(IRenderContext context, Rectangle layout)
        {
            var texture = this.m_AssetManager.Get<TextureAsset>("texture.ui.Background");
            for (var i = 0; i <= (layout.Width / texture.Texture.Width); i++)
                for (var j = 0; j <= (layout.Height / texture.Texture.Height); j++)
                {
                        var area = new Rectangle(
                        0, 
                        0,
                        (i + 1) * texture.Texture.Width > layout.Width - this.UIBorderSize ? texture.Texture.Width + layout.Width - ((i + 1) * texture.Texture.Width) - this.UIBorderSize : texture.Texture.Width, 
                        (j + 1) * texture.Texture.Height > layout.Height - this.UIBorderSize ? texture.Texture.Height + layout.Height - ((j + 1) * texture.Texture.Height) - this.UIBorderSize : texture.Texture.Height);
                        
                        this.m_2DRenderUtilities.RenderTexture(
                            context,
                            new Vector2(layout.X + (i * texture.Texture.Width), layout.Y + (j * texture.Texture.Height)),
                            texture,
                            new Vector2(area.Width, area.Height),
                            sourceArea: area);
                }
        }
        
        // Draws buttons
        // These are a fixed ratio that scales
        public void DrawButton(IRenderContext context, Rectangle layout, Button button)
        {
            // Center
            this.m_2DRenderUtilities.RenderTexture(
                context,
                new Vector2(layout.X, layout.Y),
                this.m_AssetManager.Get<TextureAsset>("texture.ui.buttons.Default"),
                new Vector2(layout.Width, layout.Height));
                
            // Text
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
            this.DrawUIBackground(context, layout);
            this.DrawUIBorder(context, layout, Side.Top);
            this.DrawUIBorder(context, layout, Side.Bottom);
            this.DrawUIBorder(context, layout, Side.Left);
            this.DrawUIBorder(context, layout, Side.Right);
            this.DrawUICorner(context, layout, Corner.TopLeft);
            this.DrawUICorner(context, layout, Corner.TopRight);
            this.DrawUICorner(context, layout, Corner.BottomLeft);
            this.DrawUICorner(context, layout, Corner.BottomRight);
        }

        public void DrawListView(IRenderContext context, Rectangle layout, ListView listView)
        {
            this.DrawUIBackground(context, layout);
            this.DrawUIBorder(context, layout, Side.Top);
            this.DrawUIBorder(context, layout, Side.Bottom);
            this.DrawUIBorder(context, layout, Side.Left);
            this.DrawUIBorder(context, layout, Side.Right);
            this.DrawUICorner(context, layout, Corner.TopLeft);
            this.DrawUICorner(context, layout, Corner.TopRight);
            this.DrawUICorner(context, layout, Corner.BottomLeft);
            this.DrawUICorner(context, layout, Corner.BottomRight);        
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
