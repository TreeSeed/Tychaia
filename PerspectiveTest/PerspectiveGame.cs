using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PerspectiveTest
{
    public class PerspectiveGame : Game
    {
        private BasicEffect m_Effect;
        private Random m_Random;
        private int m_Rotation;
        private Texture2D m_Player;
        private SpriteFont m_Font;
        private RenderTarget2D m_RenderTarget;
    
        public PerspectiveGame()
        {
            new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();
            
            this.m_Effect = new BasicEffect(GraphicsDevice);
            this.m_Random = new Random();
            
            this.m_Player = this.Content.Load<Texture2D>(@"Content\chars\player\player");
            this.m_Font = this.Content.Load<SpriteFont>(@"Content\Arial");
            
            this.m_RenderTarget = new RenderTarget2D(GraphicsDevice, 100, 40);
        }
        
        protected override void Update(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(this.m_RenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            using (var spriteBatch = new SpriteBatch(GraphicsDevice))
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(this.m_Font, "test", new Vector2(0, 0), Color.White);
                spriteBatch.End();
            }
            GraphicsDevice.SetRenderTarget(null);
            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);     

            //var rotation = 0;
            //var modelPosition = new Vector3(0, 0, 0);
            //var cameraPosition = new Vector3(-100, -100, -100);

            // Set the World matrix which defines the position of the cube
            /*
        
            // Set the View matrix which defines the camera and what it's looking at
            this.m_Effect.View = Matrix.CreateLookAt(cameraPosition, modelPosition, Vector3.Up);
        
            // Set the Projection matrix which defines how we see the scene (Field of view)
            this.m_Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 1000.0f);
        
            // Enable textures on the Cube Effect. this is necessary to texture the model
            */
            //this.m_Effect.Texture = cubeTexture;
        
            this.m_Effect.VertexColorEnabled = true;
            this.m_Effect.TextureEnabled = false;
            this.m_Effect.LightingEnabled = false;
            this.m_Effect.View = Matrix.CreateLookAt(new Vector3(0.0f, -1.0f, 1.0f), Vector3.Zero, Vector3.Up);
            this.m_Effect.Projection = Matrix.CreateOrthographicOffCenter(-2, 2, 2, -2, 1.0f, 1000.0f);
            this.m_Effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation));/*
                Matrix.CreateRotationX(MathHelper.ToRadians(this.m_Rotation));*/
            this.m_Rotation++;
        
            // Enable some pretty lights
            //this.m_Effect.EnableDefaultLighting();
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
            var indicies = new short[18]{ 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5, 4, 5, 6, 6, 5, 7 };
            
            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
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
            var textureIndicies = new short[18]{ 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5, 4, 5, 6, 6, 5, 7 };
            
            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
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
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 0, -1), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(1, 1)),
            };
            var fontIndicies = new short[6]{ 0, 1, 2, 2, 1, 3 };
            
            GraphicsDevice.BlendState = BlendState.Additive;
            foreach (var pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList,
                    fontVertexes,
                    0,   // vertex buffer offset to add to each element of the index buffer
                    4,   // number of vertices to draw
                    fontIndicies,
                    0,   // first index element to read
                    2);
            }
            GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}

