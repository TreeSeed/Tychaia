using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PerspectiveTest
{
    public class SingleCubeDemo : IRenderDemo
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
            
            var vertexes = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), Color.Black),
                new VertexPositionColor(new Vector3(0, 0, 1), Color.Blue),
                new VertexPositionColor(new Vector3(0, 1, 0), Color.Green),
                new VertexPositionColor(new Vector3(0, 1, 1), Color.Black),
                new VertexPositionColor(new Vector3(1, 0, 0), Color.Black),
                new VertexPositionColor(new Vector3(1, 0, 1), Color.Black),
                new VertexPositionColor(new Vector3(1, 1, 0), Color.Black),
                new VertexPositionColor(new Vector3(1, 1, 1), Color.White),
            };
            
            var indicies = new short[]
            {
                0, 1, 2, 1, 2, 3,
            };
            
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
                    indicies.Length / 3);
            }
        }
    }
}

