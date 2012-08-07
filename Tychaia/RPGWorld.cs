using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.MultiLevel;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Generators;
using Tychaia.Title;

namespace Tychaia
{
    public class RPGWorld : World
    {
        private ChunkManager m_ChunkManager = null;
        private Player m_Player = null;

        public RPGWorld()
            : base()
        {
            this.m_ChunkManager = new ChunkManager();
            this.m_Player = new Player(this);
            this.m_Player.Z = 0;
            this.Entities.Add(this.m_Player);
        }

        public ChunkManager ChunkManager
        {
            get { return this.m_ChunkManager; }
        }

        public override void DrawBelow(GameContext context)
        {
            context.Graphics.GraphicsDevice.Clear(Color.Black);
        }

        public override void DrawAbove(GameContext context)
        {
            XnaGraphics xna = new XnaGraphics(context);
            xna.DrawStringLeft(8, 8, "FPS: " + context.FPS, "Arial");
        }

        public override bool Update(GameContext context)
        {
            MouseState mouse = Mouse.GetState();

            // Go back to title screen if needed.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                (this.Game as RuntimeGame).SwitchWorld(new TitleWorld());
                return false;
            }

            // Update player and refocus screen.
            KeyboardState state = Keyboard.GetState();
            float mv = (float)Math.Sqrt(this.m_Player.MovementSpeed);
            if (state.IsKeyDown(Keys.W))
            {
                this.m_Player.Y -= mv;
                this.m_Player.X -= mv;
            }
            if (state.IsKeyDown(Keys.S))
            {
                this.m_Player.Y += mv;
                this.m_Player.X += mv;
            }
            if (state.IsKeyDown(Keys.A))
            {
                this.m_Player.Y += mv;
                this.m_Player.X -= mv;
            }
            if (state.IsKeyDown(Keys.D))
            {
                this.m_Player.Y -= mv;
                this.m_Player.X += mv;
            }
            (context.WorldManager as IsometricWorldManager).Focus(this.m_Player.X, this.m_Player.Y, this.m_Player.Z);

            return true; // Update entities.
        }
    }
}
