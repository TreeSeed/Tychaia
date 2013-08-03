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
    public class BackgroundCubeEntity : Entity
    {
        private int m_Distance;
        private static Random m_Random = new Random();
        private double m_ScreenX;
        private double m_ScreenY;
        private IRenderUtilities m_RenderUtilities;
        private IsometricCubeAsset m_CubeAsset;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IAssetManager m_AssetManager;
        private IIsometricifier m_Isometricifier;

        public BackgroundCubeEntity(
            IRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IChunkSizePolicy chunkSizePolicy,
            IIsometricifier isometricifier,
            bool atBottom)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_ScreenX = m_Random.NextDouble();
            this.m_ScreenY = atBottom ? 1.0 : m_Random.NextDouble();
            this.m_Distance = m_Random.Next(1, 50);
            this.m_Isometricifier = isometricifier;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_CubeAsset = null;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.m_CubeAsset == null)
            {
                this.m_CubeAsset = this.m_Isometricifier.Isometricify(
                    gameContext,
                    this.m_AssetManager.Get<TextureAsset>("texture.Grass"));
            }
        
            this.m_ScreenY -= (100.0f / m_Distance) / 5000.0f;
            this.X = (float)(this.m_ScreenX * gameContext.Window.ClientBounds.Width);
            this.Y = (float)(this.m_ScreenY * gameContext.Window.ClientBounds.Height);

            if ((int)this.Y + (int)(this.m_ChunkSizePolicy.ChunkTextureTopHeight / this.m_Distance) +
                (int)(this.m_ChunkSizePolicy.ChunkTextureSideHeight * 2.0 / this.m_Distance) < 0)
                gameContext.World.Entities.Remove(this);

            base.Update(gameContext, updateContext);
        }
        
        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (this.m_CubeAsset == null)
                return;
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    (int)this.X,
                    (int)this.Y),
                this.m_CubeAsset.TopTexture,
                size: new Vector2(
                    (int)(this.m_ChunkSizePolicy.ChunkTextureTopWidth * 2.0 / this.m_Distance),
                    (int)(this.m_ChunkSizePolicy.ChunkTextureTopHeight * 2.0 / this.m_Distance)),
                color: new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    (int)this.X + (int)(this.m_ChunkSizePolicy.ChunkTextureSideWidth * 2.0 / this.m_Distance),
                    (int)this.Y + (int)(this.m_ChunkSizePolicy.ChunkTextureTopHeight / this.m_Distance)),
                this.m_CubeAsset.LeftTexture,
                size: new Vector2(
                    (int)(this.m_ChunkSizePolicy.ChunkTextureSideWidth * 2.0 / this.m_Distance),
                    (int)(this.m_ChunkSizePolicy.ChunkTextureSideHeight * 2.0 / this.m_Distance)),
                color: new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(
                    (int)this.X,
                    (int)this.Y + (int)(this.m_ChunkSizePolicy.ChunkTextureTopHeight / this.m_Distance)),
                this.m_CubeAsset.RightTexture,
                size: new Vector2(
                    (int)(this.m_ChunkSizePolicy.ChunkTextureSideWidth * 2.0 / this.m_Distance),
                    (int)(this.m_ChunkSizePolicy.ChunkTextureSideHeight * 2.0 / this.m_Distance)),
                color: new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
        }
    }
}

