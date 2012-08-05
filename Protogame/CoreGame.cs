﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public abstract class CoreGame<InitialWorld, WorldManagerType> : Game where InitialWorld : World, new() where WorldManagerType : WorldManager, new()
    {
        protected GameContext m_GameContext = null;
        private WorldManager m_WorldManager = null;
        private int m_TotalFrames = 0;
        private float m_ElapsedTime = 0.0f;

        public World World
        {
            get
            {
                return this.m_GameContext.World;
            }
        }
        
        public CoreGame()
        {
            this.Content.RootDirectory = System.IO.Path.Combine(Protogame.World.RuntimeDirectory, "Content");
            this.m_GameContext = new GameContext
            {
                Content = this.Content,
                World = new InitialWorld(),
                Graphics = new GraphicsDeviceManager(this),
                Game = this
            };
            this.m_WorldManager = new WorldManagerType();
            this.m_GameContext.WorldManager = this.m_WorldManager;
            this.World.GameContext = this.m_GameContext;
            this.World.Game = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Window.Title = "Protogame!";

            base.Initialize();
            this.m_GameContext.Camera = new Camera(this.m_GameContext.Graphics.GraphicsDevice.Viewport.Width, this.m_GameContext.Graphics.GraphicsDevice.Viewport.Height);
            this.m_GameContext.Window = this.Window;
        }

        /// <summary>
        /// Forces the game to switch to a new world type.  Should be used sparingly as it requires
        /// discarding and reloading all entities and tilesets.
        /// </summary>
        /// <param name="w">The world to switch to.</param>
        public void SwitchWorld(World w)
        {
            if (this.World != null)
                this.World.Game = null;
            this.m_GameContext.World = w;
            this.World.GameContext = this.m_GameContext;
            this.World.Game = this;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.m_GameContext.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
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
        protected sealed override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    this.Exit();

            // Measure FPS.
            this.m_ElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (this.m_ElapsedTime >= 1000f)
            {
                this.m_GameContext.FPS = this.m_TotalFrames;
                this.m_TotalFrames = 0;
                this.m_ElapsedTime = 0;
            }

            // Update the game.
            this.m_GameContext.GameTime = gameTime;
            this.m_WorldManager.Update(this.m_GameContext);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.m_TotalFrames++;

            // Skip if we haven't yet loaded the sprite batch.
            if (this.m_GameContext.SpriteBatch == null)
                throw new ProtogameException("The sprite batch instance was not set when it came time to draw the game.  Ensure that you are calling base.LoadContent in the overridden LoadContent method of your game.");

            // Draw the game.
            this.m_GameContext.GameTime = gameTime;
            this.m_WorldManager.Draw(this.m_GameContext);

            base.Draw(gameTime);
        }
    }
}
