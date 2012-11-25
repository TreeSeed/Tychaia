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
using Tychaia.Disk;

namespace Tychaia
{
    public class RPGWorld : World
    {
        private ChunkOctree m_Octree = null;
        private Player m_Player = null;
        private ILevel m_DiskLevel = null;
        public int m_AutoSave = 0;
        public const int AUTOSAVE_LIMIT = 60 /* frames */ * 60 /* seconds */;

        public RPGWorld(LevelReference levelRef)
            : base()
        {
            this.m_DiskLevel = levelRef.Source.LoadLevel(levelRef.Name);
            this.m_Octree = new ChunkOctree();
            new Chunk(this.m_DiskLevel, this.m_Octree, 0, 0, 0);
            this.m_Player = new Player(this);
            this.m_Player.SearchForTerrain = true;
            this.Entities.Add(this.m_Player);
        }

        public ChunkOctree ChunkOctree
        {
            get { return this.m_Octree; }
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
            DebugTracker.Draw(context, this);
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
            if (state.IsKeyDown(Keys.W))
            {
                this.m_Player.Y -= mv;
                this.m_Player.X -= mv;
            }
            if (state.IsKeyDown(Keys.S) || FilteredFeatures.IsEnabled(Feature.DebugMovement))
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
            if (state.IsKeyDown(Keys.I))
            {
                this.m_Player.Z += 4f;
            }
            if (state.IsKeyDown(Keys.K))
            {
                this.m_Player.Z -= 4f;
            }
            Vector2 v = new Vector2(
                gpstate.ThumbSticks.Left.X,
                -gpstate.ThumbSticks.Left.Y
                );
            Matrix m = Matrix.CreateRotationZ(MathHelper.ToRadians(-45));
            v = Vector2.Transform(v, m);
            this.m_Player.X += v.X * mv * (float)(Math.Sqrt(2) / 1.0);
            this.m_Player.Y += v.Y * mv * (float)(Math.Sqrt(2) / 1.0);
            //this.m_Player.Z = this.GetSurfaceZ(context, this.m_Player.X, this.m_Player.Y) * Scale.CUBE_Z;
            (context.WorldManager as IsometricWorldManager).Focus(this.m_Player.X, this.m_Player.Y, this.m_Player.Z);

            // Update autosave counter.
            this.m_AutoSave++;
            if (this.m_AutoSave >= AUTOSAVE_LIMIT)
            {
                this.m_AutoSave = 0;
                if (this.m_DiskLevel != null)
                    this.m_DiskLevel.Save();
            }

            return true; // Update entities.
        }

        private float GetSurfaceZ(GameContext context, float xx, float yy)
        {
            /*int x = (int)((xx < 0 ? xx + 1 : xx) % (Chunk.Width * Scale.CUBE_X) / Scale.CUBE_X);
            int y = (int)((yy < 0 ? yy + 1 : yy) % (Chunk.Height * Scale.CUBE_Y) / Scale.CUBE_Y);
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if ((context.WorldManager as IsometricWorldManager).Chunk == null)
                return 0;
            for (int z = 0; z < Chunk.Depth; z++)
                if ((context.WorldManager as IsometricWorldManager).Chunk.m_Blocks[x, y, z] != null)
                    return z - 1;*/
            return 0;
        }
    }
}
