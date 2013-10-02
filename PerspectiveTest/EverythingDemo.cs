// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PerspectiveTest
{
    public class EverythingDemo : IRenderDemo
    {
        private BasicEffect m_Effect;
        private Random m_Random;
        private int m_Rotation;
        private Texture2D m_Player;
        private SpriteFont m_Font;
        private RenderTarget2D m_RenderTarget;
        
        public void LoadContent(Game game)
        {
            this.m_Effect = new BasicEffect(game.GraphicsDevice);
            this.m_Random = new Random();
            
            this.m_Player = game.Content.Load<Texture2D>(@"Content\chars\player\player");
            this.m_Font = game.Content.Load<SpriteFont>(@"Content\Arial");
            
            this.m_RenderTarget = new RenderTarget2D(game.GraphicsDevice, 150, 40);
        }

        public void Update(Game game)
        {
            game.GraphicsDevice.SetRenderTarget(this.m_RenderTarget);
            game.GraphicsDevice.Clear(Color.Transparent);
            using (var spriteBatch = new SpriteBatch(game.GraphicsDevice))
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(
                    this.m_Font,
                    "TigJam Australia #2\n\nClosing Presentations",
                    new Vector2(0, 0),
                    new Color(
                        this.m_Random.Next(0, 256),
                        this.m_Random.Next(0, 256),
                        this.m_Random.Next(0, 256)));
                spriteBatch.End();
            }
            
            game.GraphicsDevice.SetRenderTarget(null);
            
            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }

        public void Draw(Game game)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);     

            this.m_Effect.VertexColorEnabled = true;
            this.m_Effect.TextureEnabled = false;
            this.m_Effect.LightingEnabled = false;
            this.m_Effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 3.0f, 3.0f), Vector3.Zero, Vector3.Up);
            this.m_Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 1000.0f);
            this.m_Effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation));
            this.m_Rotation++;
            
            var vertexes = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), Color.White),
                new VertexPositionColor(new Vector3(0, 1, 0), Color.Red),
                new VertexPositionColor(new Vector3(1, 0, 0), Color.Blue),
                new VertexPositionColor(new Vector3(0, 0, 2), Color.Green),
                new VertexPositionColor(new Vector3(0, 2, 0), Color.Orange),
                new VertexPositionColor(new Vector3(2, 0, 0), Color.Purple),
                new VertexPositionColor(new Vector3(0, 0, 3), Color.Yellow),
                new VertexPositionColor(new Vector3(0, 3, 0), Color.Black),
            };
            var indicies = new short[18] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5, 4, 5, 6, 6, 5, 7 };
            
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
            
            this.m_Effect.TextureEnabled = true;
            this.m_Effect.VertexColorEnabled = false;
            this.m_Effect.Texture = this.m_Player;
        
            var textureVertexes = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0, -1, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-1, 0, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(0, 0, -2), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(0, -2, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-2, 0, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0, 0, -3), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0, -3, 0), new Vector2(1, 0)),
            };
            var textureIndicies = new short[18] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5, 4, 5, 6, 6, 5, 7 };
            
            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList,
                    textureVertexes,
                    0,   // vertex buffer offset to add to each element of the index buffer
                    8,   // number of vertices to draw
                    textureIndicies,
                    0,   // first index element to read
                    6);
            }
            
            this.m_Effect.Texture = this.m_RenderTarget;
            
            var fontVertexes = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(1.5f, 0, -1.5f), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(1.5f, 1, -1.5f), new Vector2(0, 0)),
                
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(-1, 1)),
                new VertexPositionTexture(new Vector3(-1.5f, 0, 1.5f), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(-1, 0)),
                new VertexPositionTexture(new Vector3(-1.5f, 1, 1.5f), new Vector2(0, 0)),
            };
            var fontIndicies1 = new short[6] { 0, 1, 2, 2, 1, 3 };
            var fontIndicies2 = new short[6] { 4, 5, 6, 6, 5, 7 };
            
            game.GraphicsDevice.BlendState = BlendState.Additive;
            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList,
                    fontVertexes,
                    0,   // vertex buffer offset to add to each element of the index buffer
                    4,   // number of vertices to draw
                    fontIndicies1,
                    0,   // first index element to read
                    2);
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList,
                    fontVertexes,
                    0,   // vertex buffer offset to add to each element of the index buffer
                    4,   // number of vertices to draw
                    fontIndicies2,
                    0,   // first index element to read
                    2);
            }
            
            game.GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
