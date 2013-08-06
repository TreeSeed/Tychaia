//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public class BackgroundCubeEntity : Entity
    {
        private int m_Distance;
        private static Random m_Random = new Random();
        private double m_ScreenX;
        private double m_ScreenY;
        private I3DRenderUtilities m_3DRenderUtilities;
        private TextureAsset m_GrassAsset;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IAssetManager m_AssetManager;

        public BackgroundCubeEntity(
            I3DRenderUtilities _3dRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IChunkSizePolicy chunkSizePolicy,
            bool atBottom)
        {
            this.m_3DRenderUtilities = _3dRenderUtilities;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_ScreenX = m_Random.NextDouble();
            this.m_ScreenY = atBottom ? 1.0 : m_Random.NextDouble();
            this.m_Distance = m_Random.Next(1, 50);
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_GrassAsset = this.m_AssetManager.Get<TextureAsset>("texture.Grass");
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_ScreenY -= (100.0f / m_Distance) / 5000.0f;
            this.X = (float)(5 - this.m_ScreenX * 10);//(float)(this.m_ScreenX * gameContext.Window.ClientBounds.Width);
            this.Z = (float)(5 - this.m_ScreenY * 10);//(float)(this.m_ScreenY * gameContext.Window.ClientBounds.Height);

            if (this.Z > 10)
                gameContext.World.Entities.Remove(this);

            //if ((int)this.Y + (int)(this.m_ChunkSizePolicy.CellTextureTopPixelHeight / this.m_Distance) +
            //    (int)(this.m_ChunkSizePolicy.CellTextureSidePixelHeight * 2.0 / this.m_Distance) < 0)
            //    gameContext.World.Entities.Remove(this);

            base.Update(gameContext, updateContext);
        }
        
        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
                return;
        
            if (this.m_GrassAsset == null)
                return;
            
            var vertexes = new[]
            {
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 1)),
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
            
            renderContext.EnableTextures();
            renderContext.SetActiveTexture(this.m_GrassAsset.Texture);
            renderContext.World = Matrix.CreateTranslation(new Vector3(this.X, this.Y, this.Z));
            
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
}

