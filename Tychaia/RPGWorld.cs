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
using Tychaia.Globals;
using Tychaia.Game;

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

            // Draw UI.
            xna.DrawSprite(context.Camera.Width / 2 - xna.SpriteWidth("ui.frame") / 2,
                context.Camera.Height - xna.SpriteHeight("ui.frame"),
                "ui.frame");

            // Draw debug information.
            DebugTracker.Draw(context);
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
            GamePadState gpstate = GamePad.GetState(PlayerIndex.One);
            float mv = (float)Math.Sqrt(this.m_Player.MovementSpeed);
            if (state.IsKeyDown(Keys.W) || FilteredFeatures.IsEnabled(Feature.DebugMovement))
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
            Vector2 v = new Vector2(
                gpstate.ThumbSticks.Left.X,
                -gpstate.ThumbSticks.Left.Y
                );
            Matrix m = Matrix.CreateRotationZ(MathHelper.ToRadians(-45));
            v = Vector2.Transform(v, m);
            this.m_Player.X += v.X * mv * (float)(Math.Sqrt(2) / 1.0);
            this.m_Player.Y += v.Y * mv * (float)(Math.Sqrt(2) / 1.0);
            this.m_Player.Z = this.GetSurfaceZ(context, this.m_Player.X, this.m_Player.Y) * Scale.CUBE_Z;
            (context.WorldManager as IsometricWorldManager).Focus(this.m_Player.X, this.m_Player.Y, this.m_Player.Z);

            return true; // Update entities.
        }

        private float GetSurfaceZ(GameContext context, float xx, float yy)
        {
            int x = (int)((xx < 0 ? xx + 1 : xx) % (Chunk.Width * Scale.CUBE_X) / Scale.CUBE_X);
            int y = (int)((yy < 0 ? yy + 1 : yy) % (Chunk.Height * Scale.CUBE_Y) / Scale.CUBE_Y);
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if ((context.WorldManager as IsometricWorldManager).Chunk == null)
                return 0;
            for (int z = 0; z < Chunk.Depth; z++)
                if ((context.WorldManager as IsometricWorldManager).Chunk.m_Blocks[x, y, z] != null)
                    return z - 1;
            return 0;
        }
    }
}
