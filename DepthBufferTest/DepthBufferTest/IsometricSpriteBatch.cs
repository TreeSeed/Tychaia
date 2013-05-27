using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DepthBufferTest
{
    public class IsometricSpriteBatch
    {
        private VertexPositionColorTexture[] m_Vertices;
        private short[] m_Indices;
        private int m_VertexCount = 0;
        private int m_IndexCount = 0;
        private Texture2D m_Texture = null;
        private VertexDeclaration m_Declaration = null;
        private GraphicsDevice m_GraphicsDevice = null;

        public Matrix World;
        public Matrix View;
        public Matrix Projection;
        public Effect Effect;

        public IsometricSpriteBatch(GraphicsDevice device)
        {
            this.m_GraphicsDevice = device;
            this.m_Vertices = new VertexPositionColorTexture[256];
            this.m_Indices = new short[m_Vertices.Length * 3 / 2];
        }

        public void ResetMatrices(int width, int height)
        {
            this.World = Matrix.Identity;
            this.View = new Matrix(
                1, 0, 0, 0,
                0, -1, 0, 0,
                0, 0, -1, 0,
                0, 0, 0, 1);
            this.Projection = Matrix.CreateOrthographicOffCenter(
                0, width, -height, 0, 0, 1);
        }

        public void Draw(Texture2D texture, Vector2 dst, Color color, float z)
        {
            this.Draw(
                texture,
                texture.Bounds,
                new Rectangle((int)dst.X, (int)dst.Y, texture.Bounds.Width, texture.Bounds.Height), color, z);
        }

        public void Draw(Texture2D texture, Rectangle dst, Color color, float z)
        {
            this.Draw(texture, texture.Bounds, dst, color, z);
        }

        public void Draw(Texture2D texture, Rectangle src, Rectangle dst, Color color, float z)
        {
            // Add new indicies.
            this.m_Indices[this.m_IndexCount++] = (short)(this.m_VertexCount + 0);
            this.m_Indices[this.m_IndexCount++] = (short)(this.m_VertexCount + 1);
            this.m_Indices[this.m_IndexCount++] = (short)(this.m_VertexCount + 3);
            this.m_Indices[this.m_IndexCount++] = (short)(this.m_VertexCount + 1);
            this.m_Indices[this.m_IndexCount++] = (short)(this.m_VertexCount + 2);
            this.m_Indices[this.m_IndexCount++] = (short)(this.m_VertexCount + 3);

            // Add new vertices
            this.m_Vertices[this.m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dst.Left, dst.Top, z), color, GetUV(texture, src.Left, src.Top));
            this.m_Vertices[this.m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dst.Right, dst.Top, z), color, GetUV(texture, src.Right, src.Top));
            this.m_Vertices[this.m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dst.Right, dst.Bottom, z), color, GetUV(texture, src.Right, src.Bottom));
            this.m_Vertices[this.m_VertexCount++] = new VertexPositionColorTexture(
                new Vector3(dst.Left, dst.Bottom, z), color, GetUV(texture, src.Left, src.Bottom));

            // Flush.
            if (this.m_VertexCount > 0)
            {
                EffectTechnique technique = this.Effect.CurrentTechnique;
                this.Effect.CurrentTechnique.Passes[0].Apply();
                this.m_GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                    PrimitiveType.TriangleList, this.m_Vertices, 0, this.m_VertexCount,
                    this.m_Indices, 0, this.m_IndexCount / 3);

                this.m_VertexCount = 0;
                this.m_IndexCount = 0;
            }
        }

        Vector2 GetUV(Texture2D texture, float x, float y)
        {
            return new Vector2(x / (float)texture.Width, y / (float)texture.Height);
        }
    }
}
