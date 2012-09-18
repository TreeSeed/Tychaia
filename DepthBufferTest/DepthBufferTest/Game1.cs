using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StockEffects;
using Protogame.Efficiency;

namespace DepthBufferTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        OccludingSpriteBatch occludingSpriteBatch;
        Texture2D gradient;
        Texture2D personDepth;
        Texture2D personSprite;
        int squareX = 200;
        int squareY = 200;
        float squareDepth = 1f;
        double totalFrames = 0;
        double totalTime = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            /*this.screenColorBuffer = new RenderTarget2D(this.GraphicsDevice,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);
            this.screenDepthBuffer = new RenderTarget2D(this.GraphicsDevice,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);
            this.worldColorBuffer = new RenderTarget2D(this.GraphicsDevice,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);
            this.worldDepthBuffer = new RenderTarget2D(this.GraphicsDevice,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);*/

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            occludingSpriteBatch = new OccludingSpriteBatch(GraphicsDevice);
            /*mySpriteBatch.Effect = new SpriteEffect(this.Content.Load<Effect>("SpriteEffect"));
            mergingSpriteBatch = new MySpriteBatch(GraphicsDevice);
            Effect directDepth = this.Content.Load<Effect>("DirectDepthEffect");
            directDepth.Parameters["DepthTexture"].SetValue(this.gradient);
            directDepth.CurrentTechnique.Passes[0].Apply();
            mergingSpriteBatch.Effect = new SpriteEffect(directDepth);*/

            // TODO: use this.Content to load your game content here
            gradient = this.Content.Load<Texture2D>("Gradient");
            personDepth = this.Content.Load<Texture2D>("PersonDepth");
            personSprite = this.Content.Load<Texture2D>("PersonSprite");
            occludingSpriteBatch.DepthTexture = gradient;
            /*effect = this.Content.Load<Effect>("Effect1");
            effect.Parameters["ColorScreenTexture"].SetValue(this.screenColorBuffer);
            effect.Parameters["ColorWorldTexture"].SetValue(this.screenDepthBuffer);
            effect.Parameters["DepthScreenTexture"].SetValue(this.worldColorBuffer);
            effect.Parameters["DepthWorldTexture"].SetValue(this.worldDepthBuffer);
            effect.CurrentTechnique.Passes[0].Apply();*/
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            if (state.IsKeyDown(Keys.Up))
                this.squareY -= 3;
            if (state.IsKeyDown(Keys.Down))
                this.squareY += 3;
            if (state.IsKeyDown(Keys.Left))
                this.squareX -= 3;
            if (state.IsKeyDown(Keys.Right))
                this.squareX += 3;
            if (state.IsKeyDown(Keys.Z) && this.squareDepth < 1)
                this.squareDepth = Math.Min(1f, this.squareDepth + 0.1f);
            if (state.IsKeyDown(Keys.X) && this.squareDepth > 0)
                this.squareDepth = Math.Max(0f, this.squareDepth - 0.1f);

            // Prerender
            /*this.GraphicsDevice.SetRenderTarget(depthBuffer);

            this.GraphicsDevice.SetRenderTarget(screenBuffer);
            spriteBatch.Begin();
            spriteBatch.Draw(this.gradient, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(this.square, new Vector2(this.squareX, this.squareY), Color.White);
            spriteBatch.End();
            this.GraphicsDevice.SetRenderTarget(null);*/

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            /*GraphicsDevice.DepthStencilState.DepthBufferWriteEnable = true;
            GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState.DepthBufferFunction = CompareFunction.LessEqual;*/


            // Draw the colour and depth buffers for the world.
            /*this.GraphicsDevice.SetRenderTarget(worldColorBuffer);
            this.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(this.gradient, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            this.GraphicsDevice.SetRenderTarget(worldDepthBuffer);
            this.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(this.gradient, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            // Draw the colour and depth buffers for the screen.
            this.GraphicsDevice.SetRenderTarget(screenColorBuffer);
            this.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(this.personSprite, new Vector2(this.squareX, this.squareY), Color.White);
            spriteBatch.End();
            this.GraphicsDevice.SetRenderTarget(screenDepthBuffer);
            this.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(this.personDepth, new Vector2(this.squareX, this.squareY), Color.White);
            spriteBatch.End();
            this.GraphicsDevice.SetRenderTarget(null);*/

            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, this.effect);
            //spriteBatch.Draw(worldColorBuffer, new Vector2(0, 0), Color.White);
            //spriteBatch.End();

            if (gameTime.ElapsedGameTime.TotalMilliseconds != 0)
                this.Window.Title = "FPS: " + (1000f / gameTime.ElapsedGameTime.TotalMilliseconds).ToString();
            else
                this.Window.Title = "FPS: 0";

            occludingSpriteBatch.Begin();
            occludingSpriteBatch.DrawOccluding(this.gradient, Vector2.Zero, Color.White);

            // Draw tests.
            Random r = new Random();
            for (int i = 0; i < 10000; i++)
            {
                Vector2 rand = new Vector2(r.Next(0, 800 - 32), r.Next(0, 600 - 32));
                occludingSpriteBatch.DrawOccludable(this.personSprite, rand, Color.White, i / 20000f);
            }

            // Draw player sprite.
            occludingSpriteBatch.DrawOccludable(this.personSprite, new Vector2(this.squareX, this.squareY), Color.White, squareDepth);

            occludingSpriteBatch.End();

            /*mergingSpriteBatch.ResetMatrices(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);
            mergingSpriteBatch.Draw(this.gradient, Vector2.Zero, Color.White, -1f);
            mergingSpriteBatch.Flush();

            mySpriteBatch.ResetMatrices(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);
            mySpriteBatch.Draw(this.personSprite, new Vector2(this.squareX, this.squareY), Color.White, -squareDepth);
            mySpriteBatch.Flush();*/

            base.Draw(gameTime);
        }
    }
}
