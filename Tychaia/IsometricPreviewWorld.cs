using System;
using Protogame;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tychaia
{
    public class IsometricPreviewWorld : IWorld
    {
        private IIsometricifier m_Isometricifier;
        private IRenderUtilities m_RenderUtilities;
        private IAssetManager m_AssetManager;
        
        private TextureAsset m_TextureAsset;
        private IsometricCubeAsset m_CubeAsset;
        
        private RenderTarget2D m_RenderTarget2D;
        private int m_ActiveConstructCall = 0;
        private List<Action<IGameContext, RenderTarget2D>> m_ConstructCalls;
        
        public List<IEntity> Entities { get; set; }
    
        public IsometricPreviewWorld(
            IAssetManagerProvider assetManagerProvider,
            IIsometricifier isometricifier,
            IRenderUtilities renderUtilities)
        {
            this.m_Isometricifier = isometricifier;
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            
            this.Entities = new List<IEntity>();
            
            this.m_TextureAsset = this.m_AssetManager.Get<TextureAsset>("texture.Grass");
            this.m_ConstructCalls = new List<Action<IGameContext, RenderTarget2D>>();
            this.m_ConstructCalls.Add((gameContext, target) => {
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(target);
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Red, 1.0f, 0);
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            });
            this.m_ConstructCalls.Add((gameContext, target) => {
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(target);
                int rotSize = (int)Math.Sqrt(Math.Pow(16, 2) + Math.Pow(16, 2));
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Red, 1.0f, 0);
                var spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.Draw(
                    this.m_TextureAsset.Texture,
                    new Rectangle(0, 0, rotSize, rotSize),
                    null,
                    Color.White,
                    MathHelper.ToRadians(45),
                    //new Vector2(TILE_LEFT, TILE_TOP),
                    new Vector2(-8, 8),
                    SpriteEffects.None,
                    0);
                spriteBatch.End();
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            });
            this.m_ConstructCalls.Add((gameContext, target) => {
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(target);
                var m = 
                    Matrix.CreateRotationZ(MathHelper.ToRadians(45)) *
                    Matrix.CreateScale(1, 0.7f, 1);
                int rotSize = (int)Math.Sqrt(Math.Pow(16, 2) + Math.Pow(16, 2));
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                var spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
                spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, m);
                spriteBatch.Draw(
                    this.m_TextureAsset.Texture,
                    new Rectangle(0, 0, rotSize, rotSize),
                    null,
                    Color.White);
                spriteBatch.End();
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            });
            this.m_ConstructCalls.Add((gameContext, target) => {
                // First rotate.
                int rotSize = (int)Math.Sqrt(Math.Pow(16, 2) + Math.Pow(16, 2));
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(target);
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                var spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.Draw(
                    this.m_TextureAsset.Texture,
                    new Rectangle(0, 0, rotSize, rotSize),
                    null,
                    Color.White,
                    MathHelper.ToRadians(45),
                    //new Vector2(TILE_LEFT, TILE_TOP),
                    new Vector2(-8, 8),
                    SpriteEffects.None,
                    0);
                spriteBatch.End();
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            });
            this.m_ConstructCalls.Add((gameContext, target) => {
                // First rotate.
                int rotSize = (int)Math.Sqrt(Math.Pow(16, 2) + Math.Pow(16, 2));
                var rotatedTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    31,
                    31,
                    true,
                    gameContext.Graphics.GraphicsDevice.DisplayMode.Format,
                    DepthFormat.Depth24);
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(rotatedTarget);
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                var spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.Draw(
                    this.m_TextureAsset.Texture,
                    new Rectangle(0, 0, rotSize, rotSize),
                    null,
                    Color.White,
                    MathHelper.ToRadians(45),
                    //new Vector2(TILE_LEFT, TILE_TOP),
                    new Vector2(-8, 8),
                    SpriteEffects.None,
                    0);
                spriteBatch.End();
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
    
                // Then squash.
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(target);
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.Draw(
                    rotatedTarget,
                    new Rectangle(0, 0, 32, 32),
                    new Rectangle(0, 0, rotatedTarget.Width, rotatedTarget.Height),
                    Color.White
                );
                spriteBatch.End();
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
                
                rotatedTarget.Dispose();
            });
            this.m_ConstructCalls.Add((gameContext, target) => {
                // First rotate.
                int rotSize = (int)Math.Sqrt(Math.Pow(16, 2) + Math.Pow(16, 2));
                var rotatedTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    31,
                    31);
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(rotatedTarget);
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                var spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.Draw(
                    this.m_TextureAsset.Texture,
                    new Rectangle(0, 0, rotSize, rotSize),
                    null,
                    Color.White,
                    MathHelper.ToRadians(45),
                    //new Vector2(TILE_LEFT, TILE_TOP),
                    new Vector2(-8, 8),
                    SpriteEffects.None,
                    0);
                spriteBatch.End();
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
    
                // Then squash.
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(target);
                gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.Draw(
                    rotatedTarget,
                    new Rectangle(0, 0, 32, 32),
                    new Rectangle(0, 0, rotatedTarget.Width, rotatedTarget.Height),
                    Color.White
                );
                spriteBatch.End();
                gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
                
                rotatedTarget.Dispose();
            });
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderRectangle(
                renderContext,
                gameContext.Window.ClientBounds,
                Color.CornflowerBlue,
                filled: true);
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(20, 20),
                this.m_TextureAsset);
            if (this.m_CubeAsset != null)
            {
                this.m_RenderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(100, 20),
                    this.m_CubeAsset.TopTexture);
                this.m_RenderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(200, 20),
                    this.m_CubeAsset.LeftTexture);
                this.m_RenderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(300, 20),
                    this.m_CubeAsset.RightTexture);
            }
            if (this.m_RenderTarget2D != null)
            {
                renderContext.SpriteBatch.Draw(
                    this.m_RenderTarget2D,
                    new Vector2(400, 20));
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.m_CubeAsset == null)
            {
                this.m_CubeAsset = this.m_Isometricifier.Isometricify(gameContext, this.m_TextureAsset);
            }
            
            if (this.m_RenderTarget2D == null)
            {
                this.m_RenderTarget2D = new RenderTarget2D(gameContext.Graphics.GraphicsDevice,
                    32, 32);
                this.m_ConstructCalls[this.m_ActiveConstructCall](gameContext, this.m_RenderTarget2D);
            }
            
            var mouse = Mouse.GetState();
            if (mouse.LeftPressed(this))
            {
                this.m_RenderTarget2D.Dispose();
                this.m_RenderTarget2D = null;
                this.m_ActiveConstructCall++;
                if (this.m_ActiveConstructCall >= this.m_ConstructCalls.Count)
                    this.m_ActiveConstructCall = 0;
            }
            else if (mouse.RightPressed(this))
            {
                this.m_RenderTarget2D.Dispose();
                this.m_RenderTarget2D = null;
                this.m_CubeAsset = null;
            }
        }
    }
}

