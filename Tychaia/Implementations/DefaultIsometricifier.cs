//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class DefaultIsometricifier : IIsometricifier
    {
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IContentCompiler m_ContentCompiler;
        private IAssetContentManager m_AssetContentManager;
        private IAssetManager m_AssetManager;
        private IRenderTargetFactory m_RenderTargetFactory;
        
        public DefaultIsometricifier(
            IChunkSizePolicy chunkSizePolicy,
            IContentCompiler contentCompiler,
            IAssetContentManager assetContentManager,
            IAssetManagerProvider assetManagerProvider,
            IRenderTargetFactory renderTargetFactory)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_ContentCompiler = contentCompiler;
            this.m_AssetContentManager = assetContentManager;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_RenderTargetFactory = renderTargetFactory;
        }
    
        public IsometricCubeAsset Isometricify(IGameContext gameContext, TextureAsset flatTexture)
        {
            var cached = this.m_AssetManager.TryGet<IsometricCubeAsset>(flatTexture.Name + ".isometric");
            if (cached != null)
                return cached;
        
            var original = flatTexture.Texture;
            var spriteBatch = new SpriteBatch(gameContext.Graphics.GraphicsDevice);
            var skewScale = 2;
            var skewMagic = 1.4f;
            
            #region Top Tile Generation

            // First rotate.
            int rotSize = (int)Math.Sqrt(Math.Pow(original.Width, 2) + Math.Pow(original.Height, 2));
            var rotatedTarget = this.m_RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                31,
                31
                );
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(rotatedTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            spriteBatch.Begin();
            spriteBatch.Draw(
                original,
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
            var squashedTarget = this.m_RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                this.m_ChunkSizePolicy.ChunkTextureTopWidth,
                this.m_ChunkSizePolicy.ChunkTextureTopHeight
                );
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(squashedTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            spriteBatch.Begin();
            spriteBatch.Draw(
                rotatedTarget,
                new Rectangle(0, 0, this.m_ChunkSizePolicy.ChunkTextureTopWidth, this.m_ChunkSizePolicy.ChunkTextureTopHeight),
                new Rectangle(0, 0, rotatedTarget.Width, rotatedTarget.Height),
                Color.White
            );
            spriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);
            
            rotatedTarget.Dispose();

            #endregion

            #region Side Tile Generation

            // Skew with matrix.
            Matrix m = Matrix.Identity;
            m.M11 = 1.0f;
            m.M12 = 0.7f;
            m.M21 = 0.0f;
            m.M22 = 1.0f;
            RenderTarget2D shearedLeftTarget = this.m_RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                this.m_ChunkSizePolicy.ChunkTextureSideWidth,
                this.m_ChunkSizePolicy.ChunkTextureSideHeight
                );
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(shearedLeftTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, m);
            spriteBatch.Draw(
                original,
                new Rectangle(0, 0,
                    original.Width * skewScale, original.Height * skewScale),
                null,
                new Color(63, 63, 63)
            );
            spriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);

            // Skew with matrix.
            m = Matrix.Identity;
            m.M11 = 1.0f;
            m.M12 = -0.7f;
            m.M21 = 0.0f;
            m.M22 = 1.0f;
            RenderTarget2D shearedRightTarget = this.m_RenderTargetFactory.Create(
                gameContext.Graphics.GraphicsDevice,
                this.m_ChunkSizePolicy.ChunkTextureSideWidth,
                this.m_ChunkSizePolicy.ChunkTextureSideHeight
                );
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(shearedRightTarget);
            gameContext.Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, m);
            spriteBatch.Draw(
                original,
                new Rectangle(0, (int)(original.Height * skewMagic),
                    original.Width * skewScale, original.Height * skewScale),
                null,
                new Color(127, 127, 127)
            );
            spriteBatch.End();
            gameContext.Graphics.GraphicsDevice.SetRenderTarget(null);

            #endregion
            
            if (squashedTarget == null)
                throw new InvalidOperationException();
            if (shearedLeftTarget == null)
                throw new InvalidOperationException();
            if (shearedRightTarget == null)
                throw new InvalidOperationException();

            squashedTarget.ContentLost += (sender, e) => { throw new InvalidOperationException(); };
            shearedLeftTarget.ContentLost += (sender, e) => { throw new InvalidOperationException(); };
            shearedRightTarget.ContentLost += (sender, e) => { throw new InvalidOperationException(); };

            this.m_AssetManager.Save(
                new TextureAsset(
                    this.m_ContentCompiler,
                    this.m_AssetContentManager,
                    flatTexture.Name + ".isometric.top",
                    squashedTarget));
            this.m_AssetManager.Save(
                new TextureAsset(
                    this.m_ContentCompiler,
                    this.m_AssetContentManager,
                    flatTexture.Name + ".isometric.sideL",
                    shearedRightTarget));
            this.m_AssetManager.Save(
                new TextureAsset(
                    this.m_ContentCompiler,
                    this.m_AssetContentManager,
                    flatTexture.Name + ".isometric.sideR",
                    shearedLeftTarget));
            var isometricCubeAsset = new IsometricCubeAsset(
                this.m_AssetManager,
                flatTexture.Name + ".isometric",
                flatTexture.Name + ".isometric.top",
                flatTexture.Name + ".isometric.sideL",
                flatTexture.Name + ".isometric.sideR");
            this.m_AssetManager.Save(isometricCubeAsset);
            return isometricCubeAsset;
        }
    }
}

