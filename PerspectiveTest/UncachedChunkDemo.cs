using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PerspectiveTest
{
    public class UncachedChunkDemo : IRenderDemo
    {
        private BasicEffect m_Effect;
        private Random m_Random;
        private int m_Rotation;
        private Texture2D m_Player;
        private RenderTarget2D m_RenderTarget;
        
        public void LoadContent(Game game)
        {
            this.m_Effect = new BasicEffect(game.GraphicsDevice);
            this.m_Random = new Random();
            
            this.m_Player = game.Content.Load<Texture2D>(@"Content\chars\player\player");
        }

        public void Update(Game game)
        {
        }

        public void Draw(Game game)
        {
            game.GraphicsDevice.Clear(Color.GreenYellow);
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            this.m_Effect.VertexColorEnabled = true;
            this.m_Effect.TextureEnabled = false;
            this.m_Effect.LightingEnabled = false;
            this.m_Effect.View = Matrix.CreateLookAt(new Vector3(0.0f, -10.0f, 10.0f), Vector3.Zero, Vector3.Up);
            this.m_Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 1000.0f);
            this.m_Effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation));
            this.m_Rotation++;
            
            var vertexes = new VertexPositionColor[8 * 8 * 8];
            for (var x = 0; x < 8; x++)
            for (var y = 0; y < 8; y++)
            for (var z = 0; z < 8; z++)
            {
                vertexes[x + y * 8 + z * 64] = new VertexPositionColor(new Vector3(x, y, z), new Color(new Vector3(x * 16, y * 16, z * 16)));
            }
            
            var indicies = new int[8 * 8 * 8 * 6 * 6];
            for (var x = 0; x < 8; x++)
            for (var y = 0; y < 8; y++)
            for (var z = 0; z < 8; z++)
            {
                indicies[(x + y * 8 + z * 64) * 36 +  0] = (x + 0) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  1] = (x + 1) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  2] = (x + 0) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  3] = (x + 0) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  4] = (x + 1) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  5] = (x + 1) + (y + 1) * 8 + (z + 0) * 64;
                
                indicies[(x + y * 8 + z * 64) * 36 +  6] = (x + 0) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  7] = (x + 1) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  8] = (x + 0) + (y + 1) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 +  9] = (x + 0) + (y + 1) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 10] = (x + 1) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 11] = (x + 1) + (y + 1) * 8 + (z + 1) * 64;
                
                indicies[(x + y * 8 + z * 64) * 36 + 12] = (x + 0) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 13] = (x + 0) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 14] = (x + 0) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 15] = (x + 0) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 16] = (x + 0) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 17] = (x + 0) + (y + 1) * 8 + (z + 1) * 64;
                
                indicies[(x + y * 8 + z * 64) * 36 + 18] = (x + 1) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 19] = (x + 1) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 20] = (x + 1) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 21] = (x + 1) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 22] = (x + 1) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 23] = (x + 1) + (y + 1) * 8 + (z + 1) * 64;
                
                indicies[(x + y * 8 + z * 64) * 36 + 24] = (x + 0) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 25] = (x + 0) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 26] = (x + 1) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 27] = (x + 1) + (y + 0) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 28] = (x + 0) + (y + 0) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 29] = (x + 1) + (y + 0) * 8 + (z + 1) * 64;
                
                indicies[(x + y * 8 + z * 64) * 36 + 30] = (x + 0) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 31] = (x + 0) + (y + 1) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 32] = (x + 1) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 33] = (x + 1) + (y + 1) * 8 + (z + 0) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 34] = (x + 0) + (y + 1) * 8 + (z + 1) * 64;
                indicies[(x + y * 8 + z * 64) * 36 + 35] = (x + 1) + (y + 1) * 8 + (z + 1) * 64;
            }

            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    vertexes,
                    0,   // vertex buffer offset to add to each element of the index buffer
                    8,   // number of vertices to draw
                    indicies,
                    0,   // first index element to read
                    6);
            }
        }
    }
}

