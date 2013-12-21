// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Tychaia.Asset;
using Tychaia.Globals;

namespace Tychaia
{
    public class ClientRuntimeChunk : RuntimeChunk
    {
        private readonly TextureAtlasAsset m_TextureAtlasAsset;

        private readonly EffectAsset m_TerrainEffectAsset;

        public ClientRuntimeChunk(
            ILevel level,
            ChunkOctree octree,
            IChunkFactory chunkFactory,
            IFilteredConsole filteredConsole,
            IFilteredFeatures filteredFeatures,
            IChunkSizePolicy chunkSizePolicy,
            IAssetManagerProvider assetManagerProvider,
            IChunkGenerator chunkGenerator,
            long x,
            long y,
            long z) : base(
                level,
                octree,
                chunkFactory,
                filteredConsole,
                filteredFeatures,
                chunkSizePolicy,
                assetManagerProvider,
                chunkGenerator,
                x,
                y,
                z)
        {
            this.m_TextureAtlasAsset = this.AssetManager.Get<TextureAtlasAsset>("atlas");
            this.m_TerrainEffectAsset = this.AssetManager.Get<EffectAsset>("effect.Lighting");
        }
        
        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;

            if (this.GraphicsEmpty)
                return;

            if (this.Generated && this.VertexBuffer == null && this.IndexBuffer == null)
                this.CalculateBuffers(renderContext);

            if (this.VertexBuffer != null && this.IndexBuffer != null)
            {
                renderContext.PushEffect(this.m_TerrainEffectAsset.Effect);

                renderContext.EnableTextures();
                renderContext.SetActiveTexture(this.m_TextureAtlasAsset.TextureAtlas.Texture);
                renderContext.GraphicsDevice.Indices = this.IndexBuffer;
                renderContext.GraphicsDevice.SetVertexBuffer(this.VertexBuffer);
                renderContext.World = Matrix.CreateScale(32);
                foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    renderContext.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.VertexBuffer.VertexCount, 0, this.IndexBuffer.IndexCount / 3);
                }

                renderContext.PopEffect();
            }
        }

        /// <summary>
        /// Calculates the vertex and index buffers for rendering.
        /// </summary>
        public void CalculateBuffers(IRenderContext renderContext)
        {
            if (this.GeneratedVertexes.Length == 0)
            {
                this.GraphicsEmpty = true;
                return;
            }
            
            this.VertexBuffer = new VertexBuffer(
                renderContext.GraphicsDevice,
                VertexPositionTexture.VertexDeclaration,
                this.GeneratedVertexes.Length,
                BufferUsage.WriteOnly);
            this.VertexBuffer.SetData(this.GeneratedVertexes);
            this.IndexBuffer = new IndexBuffer(
                renderContext.GraphicsDevice,
                typeof(int),
                this.GeneratedIndices.Length,
                BufferUsage.WriteOnly);
            this.IndexBuffer.SetData(this.GeneratedIndices);
        }
    }
}
