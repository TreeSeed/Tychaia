// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public class BackgroundCubeEntity : Entity
    {
        private static readonly Random m_Random = new Random();
        private readonly int m_Distance;
        private readonly TextureAsset m_GrassAsset;
        private readonly float m_Rotation;

        public BackgroundCubeEntity(
            IAssetManagerProvider assetManagerProvider,
            bool atBottom)
        {
            this.m_Distance = m_Random.Next(1, 50);
            this.m_Rotation = m_Random.Next(0, 360);
            this.m_GrassAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Grass");

            this.X = (float)(m_Random.NextDouble() - 0.5) * 25;
            this.Z = (float)(m_Random.NextDouble() - 0.5) * 25;
            if (atBottom)
                this.Y = 10;
            else
                this.Y = ((float)m_Random.NextDouble() * 60) - 50;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.Y -= (100.0f / this.m_Distance) / 50.0f;
            if (this.Y < -50)
                gameContext.World.Entities.Remove(this);

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
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 1))
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
            renderContext.World =
                Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation)) *
                Matrix.CreateTranslation(new Vector3(this.X, this.Y, this.Z));

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertexes,
                    0, // vertex buffer offset to add to each element of the index buffer
                    8, // number of vertices to draw
                    indicies,
                    0, // first index element to read
                    indicies.Length / 3);
            }
        }
    }
}
