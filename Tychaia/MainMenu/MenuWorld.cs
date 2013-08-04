//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class MenuWorld : IWorld
    {
        private List<Button> m_Buttons = new List<Button>();
        protected static Random m_Random = new Random();
        public static int m_StaticSeed = 6294563;
        protected IWorld m_TargetWorld = null;
        private int m_PreviousX = 800;
        private int m_MenuItemY = 300;
        private ScatterBackground m_ScatterBackground;
        protected IAssetManager m_AssetManager = null;
        private TitleMenu m_TitleMenu;
        private IRenderUtilities m_RenderUtilities;
        private FontAsset m_TitleFont;
        private FontAsset m_DefaultFont;
        private TextureAsset m_PlayerTexture;
        protected IGameContext m_GameContext;
        private IBackgroundCubeEntityFactory m_BackgroundCubeEntityFactory;
        private CanvasEntity m_CanvasEntity;
        
        public List<IEntity> Entities { get; private set; }
        
        public MenuWorld(
            IRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_BackgroundCubeEntityFactory = backgroundCubeEntityFactory;
            this.m_TitleFont = this.m_AssetManager.Get<FontAsset>("font.Title");
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_PlayerTexture = this.m_AssetManager.Get<TextureAsset>("chars.player.Player");
            
            this.Entities = new List<IEntity>();
            
            this.m_CanvasEntity = new CanvasEntity(skin);
            this.m_CanvasEntity.Canvas = new Canvas();
            this.m_CanvasEntity.Canvas.SetChild(this.m_TitleMenu = new TitleMenu());
            // Don't add the canvas to the entities list; that way we can explicitly
            // order it's depth.
        }
        
        public void Dispose()
        {
        }
        
        protected void AddMenuItem(LanguageAsset language, Action handler)
        {
            this.m_TitleMenu.AddChild(language.Value, (sender, e) => { handler(); });
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderRectangle(
                renderContext,
                gameContext.Window.ClientBounds,
                Color.Black,
                filled: true);
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            this.m_CanvasEntity.Render(gameContext, renderContext);
        
            this.m_RenderUtilities.RenderText(
                renderContext,
                new Vector2(gameContext.Window.ClientBounds.Center.X, 50),
                "Tychaia",
                this.m_TitleFont,
                horizontalAlignment: HorizontalAlignment.Center);
                
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    gameContext.Window.ClientBounds.Center.X,
                    gameContext.Window.ClientBounds.Center.Y),
                this.m_PlayerTexture);

            if (this.m_AssetManager != null && this.m_AssetManager.IsRemoting)
            {
                this.m_RenderUtilities.RenderText(
                    renderContext,
                    new Vector2(gameContext.Window.ClientBounds.Center.X, 10),
                    "Asset Manager: " + this.m_AssetManager.Status,
                    this.m_DefaultFont,
                    horizontalAlignment: HorizontalAlignment.Center);
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_GameContext = gameContext;
            
            if (this.m_ScatterBackground == null && gameContext.FrameCount > 60)
                this.m_ScatterBackground = new ScatterBackground(this.m_BackgroundCubeEntityFactory, this);
            if (this.m_ScatterBackground != null)
                this.m_ScatterBackground.Update(this);

            if (this.m_TargetWorld != null)
                gameContext.SwitchWorld(this.m_TargetWorld);
                
            this.m_CanvasEntity.Update(gameContext, updateContext);
        }
    }
}

