// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Asset;

namespace Tychaia
{
    public class MenuWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        private readonly IBackgroundCubeEntityFactory m_BackgroundCubeEntityFactory;
        private readonly CanvasEntity m_CanvasEntity;
        private readonly FontAsset m_DefaultFont;
        private readonly TextureAsset m_PlayerTexture;
#if DEBUG
        private readonly TextureAtlasAsset m_TextureAtlas;
#endif
        private readonly FontAsset m_TitleFont;
        private readonly TitleMenu m_TitleMenu;
        private int m_Rotation;
        private ScatterBackground m_ScatterBackground;

        public MenuWorld(
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.AssetManager = assetManagerProvider.GetAssetManager();
            this.m_BackgroundCubeEntityFactory = backgroundCubeEntityFactory;
            this.m_TitleFont = this.AssetManager.Get<FontAsset>("font.Title");
            this.m_DefaultFont = this.AssetManager.Get<FontAsset>("font.Default");
            this.m_PlayerTexture = this.AssetManager.Get<TextureAsset>("chars.player.Player");
#if DEBUG
            this.m_TextureAtlas = this.AssetManager.Get<TextureAtlasAsset>("atlas");
#endif

            this.Entities = new List<IEntity>();

            this.m_CanvasEntity = new CanvasEntity(skin) { Canvas = new Canvas() };
            this.m_CanvasEntity.Canvas.SetChild(this.m_TitleMenu = new TitleMenu());
            
            // Don't add the canvas to the entities list; that way we can explicitly
            // order it's depth.
        }
        
        public List<IEntity> Entities { get; private set; }
        protected IAssetManager AssetManager { get; private set; }
        protected IGameContext GameContext { get; private set; }
        protected IWorld TargetWorld { get; set; }

        public void Dispose()
        {
        }

        public virtual void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                renderContext.GraphicsDevice.Clear(Color.Black);

                renderContext.View = Matrix.CreateLookAt(
                    Vector3.Transform(
                        new Vector3(0.0f, 10.0f, 10.0f),
                        Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation / 10f))),
                    Vector3.Zero,
                    Vector3.Up);
                renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    renderContext.GraphicsDevice.Viewport.Width / (float)renderContext.GraphicsDevice.Viewport.Height,
                    1.0f,
                    1000.0f);
                this.m_Rotation++;
            }
        }

        public virtual void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
                return;

            this.m_CanvasEntity.Render(gameContext, renderContext);

            this.m_2DRenderUtilities.RenderText(
                renderContext,
                new Vector2(gameContext.Window.ClientBounds.Center.X, 50),
                "Tychaia",
                this.m_TitleFont,
                HorizontalAlignment.Center);

            this.m_2DRenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    gameContext.Window.ClientBounds.Center.X,
                    gameContext.Window.ClientBounds.Center.Y),
                this.m_PlayerTexture);

#if DEBUG
            this.m_2DRenderUtilities.RenderTexture(
                renderContext,
                new Vector2(10, 10), 
                this.m_TextureAtlas.TextureAtlas);
#endif
        }

        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.GameContext = gameContext;

            if (this.m_ScatterBackground == null && gameContext.FrameCount > 60)
                this.m_ScatterBackground = new ScatterBackground(this.m_BackgroundCubeEntityFactory, this);
            if (this.m_ScatterBackground != null)
                this.m_ScatterBackground.Update(this);

            if (this.TargetWorld != null)
                gameContext.SwitchWorld(this.TargetWorld);

            this.m_CanvasEntity.Update(gameContext, updateContext);
        }

        protected void AddMenuItem(LanguageAsset language, Action handler)
        {
            this.m_TitleMenu.AddChild(language.Value, (sender, e) => handler());
        }
    }
}
