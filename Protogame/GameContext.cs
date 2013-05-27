using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class GameContext
    {
        public ContentManager Content { get; set; }
        public GraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public World World { get; set; }
        public Dictionary<string, Texture2D> Textures { get; set; }
        public Dictionary<string, SpriteFont> Fonts { get; set; }
        public Dictionary<string, SoundEffect> Sounds { get; set; }
        public Dictionary<string, Effect> Effects { get; set; }
        public GameTime GameTime { get; set; }
        public Camera Camera { get; set; }
        public GameWindow Window { get; set; }
        public int FPS { get; internal set; }
        public Game Game { get; internal set; }
        public WorldManager WorldManager { get; internal set; }
        public int FrameCount { get; internal set; }

        internal GameContext()
        {
            this.Textures = new Dictionary<string, Texture2D>();
            this.Sounds = new Dictionary<string, SoundEffect>();
            this.Fonts = new Dictionary<string, SpriteFont>();
            this.Effects = new Dictionary<string, Effect>();
        }

        public Rectangle ScreenBounds
        {
            get { return this.Window.ClientBounds; }
        }

        public void SetScreenSize(int width, int height)
        {
            this.Graphics.PreferredBackBufferWidth = 1024; // GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.Graphics.PreferredBackBufferHeight = 768; // GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        public void LoadFont(string name)
        {
            this.Fonts.Add(name, this.Content.Load<SpriteFont>(name.Replace('.', '/')));
        }

        public void LoadTexture(string name)
        {
            this.Textures.Add(name, this.Content.Load<Texture2D>(name.Replace('.', '/')));
        }

        public void LoadTextureAnim(string name, int end)
        {
            for (int i = 1; i <= end; i++)
                this.LoadTexture(name + i);
        }

        public void LoadAudio(string name)
        {
            this.Sounds.Add(name, this.Content.Load<SoundEffect>(name.Replace('.', '/')));
        }

        public void LoadEffect(string name)
        {
            try
            {
                this.Effects.Add(name, this.Content.Load<Effect>(name.Replace('.', '/')));
            }
            catch (Exception)
            {
                this.Effects.Add(name, null);
            }
        }

        public void EndSpriteBatch()
        {
            this.SpriteBatch.End();
        }

        public void StartSpriteBatch()
        {
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this.Camera.GetTransformationMatrix());
        }
    }
}
