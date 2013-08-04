//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Globals;

namespace Tychaia
{
    public class DefaultIsometricRenderUtilities : IIsometricRenderUtilities
    {
        private IFilteredFeatures m_FilteredFeatures;
        private IRenderingBuffers m_RenderingBuffers;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IRenderUtilities m_RenderUtilities;
    
        public DefaultIsometricRenderUtilities(
            IFilteredFeatures filteredFeatures,
            IRenderingBuffers renderingBuffers,
            IChunkSizePolicy chunkSizePolicy,
            IRenderUtilities renderUtilities)
        {
            this.m_FilteredFeatures = filteredFeatures;
            this.m_RenderingBuffers = renderingBuffers;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_RenderUtilities = renderUtilities;
        }
        
        public void RenderEntity(
            IRenderContext renderContext,
            TychaiaGameWorld gameWorld,
            IsometricEntity entity,
            IsometricCamera camera,
            OccludingSpriteBatch occludingSpriteBatch,
            TextureAsset textureAsset)
        {
            if (renderContext == null) throw new ArgumentNullException("renderContext");
            if (gameWorld == null) throw new ArgumentNullException("gameWorld");
            if (entity == null) throw new ArgumentNullException("entity");
            if (camera == null) throw new ArgumentNullException("camera");
            if (occludingSpriteBatch == null) throw new ArgumentNullException("occludingSpriteBatch");
            if (textureAsset == null) throw new ArgumentNullException("textureAsset");
            if (!this.m_FilteredFeatures.IsEnabled(Feature.RenderEntities))
                return;

            var pos = camera.TranslatePoint(entity.X, entity.Y, entity.Z);
            if (this.m_RenderingBuffers.DepthBuffer != null && this.m_FilteredFeatures.IsEnabled(Feature.IsometricOcclusion))
            {
                float depth = ((
                    ((int)((entity.X < 0) ? this.m_ChunkSizePolicy.ChunkCellWidth : 0) + (entity.X / this.m_ChunkSizePolicy.CellVoxelWidth) % this.m_ChunkSizePolicy.ChunkCellWidth) +
                    ((int)((entity.Y < 0) ? this.m_ChunkSizePolicy.ChunkCellHeight : 0) + (entity.Y / this.m_ChunkSizePolicy.CellVoxelHeight) % this.m_ChunkSizePolicy.ChunkCellHeight) +
                    ((int)((entity.Z < 0) ? this.m_ChunkSizePolicy.ChunkCellDepth : 0) + ((entity.Z / this.m_ChunkSizePolicy.CellVoxelDepth) - 1) % this.m_ChunkSizePolicy.ChunkCellDepth)) / 255f);
                occludingSpriteBatch.DrawOccludable(
                    textureAsset.Texture,
                    new Rectangle((int)(pos.X - entity.ImageOffsetX), (int)(pos.Y - entity.ImageOffsetY),
                    textureAsset.Texture.Width, textureAsset.Texture.Height),
                    Color.White,
                    depth);
            }
            else
            {
                this.m_RenderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(pos.X - entity.ImageOffsetX, pos.Y - entity.ImageOffsetY),
                    textureAsset);
            }
        }
    }
}

