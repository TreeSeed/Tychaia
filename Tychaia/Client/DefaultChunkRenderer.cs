// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Tychaia.Asset;
using Tychaia.Runtime;

namespace Tychaia.Client
{
    public class DefaultChunkRenderer : IChunkRenderer
    {
        private readonly TextureAtlasAsset m_TextureAtlasAsset;

        private readonly EffectAsset m_TerrainEffectAsset;

        public DefaultChunkRenderer(IAssetManagerProvider assetManagerProvider)
        {
            this.m_TextureAtlasAsset = assetManagerProvider.GetAssetManager().Get<TextureAtlasAsset>("atlas");
            this.m_TerrainEffectAsset =
                assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Lighting");
        }

        /// <summary>
        /// Renders the specified chunk with the specified rendering context.
        /// </summary>
        /// <param name="renderContext">The rendering context.</param>
        /// <param name="runtimeChunk">The runtime chunk to render.</param>
        public void Render(IRenderContext renderContext, RuntimeChunk runtimeChunk)
        {
            if (!renderContext.Is3DContext)
                return;

            if (runtimeChunk.GraphicsEmpty)
                return;

            if (runtimeChunk.Generated && runtimeChunk.VertexBuffer == null && runtimeChunk.IndexBuffer == null)
                this.CalculateBuffers(renderContext, runtimeChunk);

            if (runtimeChunk.VertexBuffer != null && runtimeChunk.IndexBuffer != null)
            {
                renderContext.PushEffect(this.m_TerrainEffectAsset.Effect);

                renderContext.EnableTextures();
                renderContext.SetActiveTexture(this.m_TextureAtlasAsset.TextureAtlas.Texture);
                renderContext.GraphicsDevice.Indices = runtimeChunk.IndexBuffer;
                renderContext.GraphicsDevice.SetVertexBuffer(runtimeChunk.VertexBuffer);
                renderContext.World = Matrix.CreateScale(32);
                foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    renderContext.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        0,
                        0,
                        runtimeChunk.VertexBuffer.VertexCount,
                        0,
                        runtimeChunk.IndexBuffer.IndexCount / 3);
                }

                renderContext.PopEffect();
            }
        }

        /// <summary>
        /// Calculates the vertex and index buffers for rendering.
        /// </summary>
        private void CalculateBuffers(IRenderContext renderContext, RuntimeChunk runtimeChunk)
        {
            if (runtimeChunk.GeneratedVertexes.Length == 0)
            {
                runtimeChunk.GraphicsEmpty = true;
                return;
            }

            runtimeChunk.VertexBuffer = new VertexBuffer(
                renderContext.GraphicsDevice,
                VertexPositionTexture.VertexDeclaration,
                runtimeChunk.GeneratedVertexes.Length,
                BufferUsage.WriteOnly);
            runtimeChunk.VertexBuffer.SetData(runtimeChunk.GeneratedVertexes);
            runtimeChunk.IndexBuffer = new IndexBuffer(
                renderContext.GraphicsDevice,
                typeof(int),
                runtimeChunk.GeneratedIndices.Length,
                BufferUsage.WriteOnly);
            runtimeChunk.IndexBuffer.SetData(runtimeChunk.GeneratedIndices);
        }
    }
}