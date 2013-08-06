//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;
using Microsoft.Xna.Framework.Graphics;

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
        private I2DRenderUtilities m_2DRenderUtilities;
        private FontAsset m_TitleFont;
        private FontAsset m_DefaultFont;
        private TextureAsset m_PlayerTexture;
        protected IGameContext m_GameContext;
        private IBackgroundCubeEntityFactory m_BackgroundCubeEntityFactory;
        private CanvasEntity m_CanvasEntity;
        private int m_Rotation;
        
        public List<IEntity> Entities { get; private set; }
        
        public MenuWorld(
            I2DRenderUtilities _2dRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
        {
            this.m_2DRenderUtilities = _2dRenderUtilities;
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
            if (renderContext.Is3DContext)
            {
                renderContext.GraphicsDevice.Clear(Color.CornflowerBlue);
                
                renderContext.EnableVertexColors();
                (renderContext.Effect as BasicEffect).LightingEnabled = false;
                renderContext.View = Matrix.CreateLookAt(new Vector3(0.0f, -10.0f, 10.0f), Vector3.Zero, Vector3.Up);
                renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 1000.0f);
                renderContext.World = Matrix.CreateTranslation(new Vector3(-0.5f, -0.5f, -0.5f)) *
                    Matrix.CreateRotationX(MathHelper.ToRadians(this.m_Rotation)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation));
                this.m_Rotation++;
            
                var vertexes = new VertexPositionColor[]
                {
                    new VertexPositionColor(new Vector3(0, 0, 0), Color.Black),
                    new VertexPositionColor(new Vector3(0, 0, 1), Color.Blue),
                    new VertexPositionColor(new Vector3(0, 1, 0), Color.Green),
                    new VertexPositionColor(new Vector3(0, 1, 1), Color.Yellow),
                    new VertexPositionColor(new Vector3(1, 0, 0), Color.Purple),
                    new VertexPositionColor(new Vector3(1, 0, 1), Color.Red),
                    new VertexPositionColor(new Vector3(1, 1, 0), Color.Orange),
                    new VertexPositionColor(new Vector3(1, 1, 1), Color.White),
                };
                
                var indicies = new short[]
                {
                    0, 2, 1, 1, 2, 3,
                    4, 5, 6, 5, 7, 6,
                    0, 4, 6, 0, 6, 2,
                    1, 7, 5, 1, 3, 7,
                    0, 1, 4, 5, 4, 1,
                    6, 3, 2, 7, 3, 6
                };
                
                foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                
                    renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        vertexes,
                        0,   // vertex buffer offset to add to each element of the index buffer
                        8,   // number of vertices to draw
                        indicies,
                        0,   // first index element to read
                        indicies.Length / 3);
                }
            }
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
                return;
                
            this.m_CanvasEntity.Render(gameContext, renderContext);
        
            this.m_2DRenderUtilities.RenderText(
                renderContext,
                new Vector2(gameContext.Window.ClientBounds.Center.X, 50),
                "Tychaia",
                this.m_TitleFont,
                horizontalAlignment: HorizontalAlignment.Center);
                
            this.m_2DRenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    gameContext.Window.ClientBounds.Center.X,
                    gameContext.Window.ClientBounds.Center.Y),
                this.m_PlayerTexture);

            if (this.m_AssetManager != null && this.m_AssetManager.IsRemoting)
            {
                this.m_2DRenderUtilities.RenderText(
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

