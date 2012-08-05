﻿using System;
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
        private int m_TargetX = 0;
        private int m_TargetY = 0;
        private int m_CenterX = 0;
        private int m_CenterY = 0;

        public RPGWorld()
            : base()
        {
            this.m_ChunkManager = new ChunkManager();
            this.m_Player = new Player(this);
            this.m_Player.WorldZ = 0;
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

        private bool m_Regenerated = false;

        public void Focus(GameContext context, int x, int y)
        {
            this.m_CenterX = (context.Camera.Width / Tileset.TILESET_CELL_WIDTH) / 2;
            this.m_CenterY = (context.Camera.Height / Tileset.TILESET_CELL_HEIGHT) / 2;
            this.m_TargetX = x * Tileset.TILESET_CELL_WIDTH - (context.Camera.Width / 2);// (2 ));
            this.m_TargetY = y * Tileset.TILESET_CELL_HEIGHT - (context.Camera.Height / 2);// (2 ));
            context.Camera.X = this.m_TargetX % 16;
            context.Camera.Y = this.m_TargetY % 16;
            /*(this.Tileset as ChunkTileset).RefreshChunkReference(
                this.m_TargetX,
                this.m_TargetY);*/
        }

        public Point GetCenter()
        {
            return new Point(
                this.m_CenterX,
                this.m_CenterY
                );
        }

#if DISABLED
        private int SurfacePointOnTerrain(int x, int y)
        {
            Chunk c = this.m_DynamicLoader.LocateAtPoint(x, y);
            if (c == null) return -1;
            for (int i = 0; i < c.m_Blocks.GetLength(2); i++)
                if (c.m_Blocks[x - c.GlobalX, y - c.GlobalY, i] != null)
                    return i - 1;
            return -1;
        }

        private bool EntityCanMoveIntoPosition(ChunkEntity entity, int x, int y)
        {
            // Get the current Z position.
            Chunk c = this.m_DynamicLoader.LocateAtPoint(x, y);
            if (entity.WorldZ < 0 || entity.WorldZ > 63)
                return false;
            if (c.m_Blocks[x - c.GlobalX, y - c.GlobalY, entity.WorldZ] == null ||
                c.m_Blocks[x - c.GlobalX, y - c.GlobalY, entity.WorldZ].Transparent)
                return true;
            if (entity.WorldZ != 0 && (c.m_Blocks[x - c.GlobalX, y - c.GlobalY, entity.WorldZ - 1] == null ||
                                       c.m_Blocks[x - c.GlobalX, y - c.GlobalY, entity.WorldZ - 1].Transparent))
            {
                entity.WorldZ -= 1;
                return true;
            }
            if (!c.m_Blocks[x - c.GlobalX, y - c.GlobalY, entity.WorldZ].Impassable)
                return true;
            if (entity.WorldZ != 0 && !c.m_Blocks[x - c.GlobalX, y - c.GlobalY, entity.WorldZ - 1].Impassable)
            {
                entity.WorldZ -= 1;
                return true;
            }
            return false;
        }
#endif

        private bool waspressed = false;

        public override bool Update(GameContext context)
        {
            MouseState mouse = Mouse.GetState();

            // Go back to title screen if needed.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                (this.Game as RuntimeGame).SwitchWorld(new TitleWorld());
                return false;
            }

#if DISABLED
            // Player should fall (TODO: Move this into a gravity handler for chunk entities).
            int tz = this.SurfacePointOnTerrain(this.m_Player.WorldX, this.m_Player.WorldY);
            if (this.m_Player.WorldZ < tz && tz != -1)
                this.m_Player.WorldZ = tz;
#endif

            // Update player and refocus screen.
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))// && this.EntityCanMoveIntoPosition(this.m_Player, this.m_Player.WorldX, this.m_Player.WorldY - 1))
            {
                if (!waspressed)
                    (context.WorldManager as IsometricWorldManager).Chunk = (context.WorldManager as IsometricWorldManager).Chunk.Up;
                waspressed = true;
                //this.m_Player.Y -= 16;
            }
            else if (state.IsKeyDown(Keys.Down))// && this.EntityCanMoveIntoPosition(this.m_Player, this.m_Player.WorldX, this.m_Player.WorldY + 1))
            {
                if (!waspressed)
                    (context.WorldManager as IsometricWorldManager).Chunk = (context.WorldManager as IsometricWorldManager).Chunk.Down;
                waspressed = true;
                //this.m_Player.Y += 16;
            }
            else if (state.IsKeyDown(Keys.Left))// && this.EntityCanMoveIntoPosition(this.m_Player, this.m_Player.WorldX - 1, this.m_Player.WorldY))
            {
                if (!waspressed)
                    (context.WorldManager as IsometricWorldManager).Chunk = (context.WorldManager as IsometricWorldManager).Chunk.Left;
                waspressed = true;
                //this.m_Player.X -= 16;
            }
            else if (state.IsKeyDown(Keys.Right))// && this.EntityCanMoveIntoPosition(this.m_Player, this.m_Player.WorldX + 1, this.m_Player.WorldY))
            {
                if (!waspressed)
                    (context.WorldManager as IsometricWorldManager).Chunk = (context.WorldManager as IsometricWorldManager).Chunk.Right;
                waspressed = true;
                //this.m_Player.X += 16;
            }
            else
                waspressed = false;
            this.Focus(context, this.m_Player.WorldX, this.m_Player.WorldY);
            context.Window.Title = "At " + ((int)Math.Floor(this.m_TargetX / (double)16)) + ", " + ((int)Math.Floor(this.m_TargetY / (double)16) + " depth " + this.RenderDepthValue);

#if DISABLED
            // Check to see if we should regenerate.
            KeyboardState keystate = Keyboard.GetState();
            if (keystate.IsKeyDown(Keys.R))
            {
                if (!this.m_Regenerated)
                {
                    Log.WriteLine("Starting generation...");
                    this.m_DynamicLoader.Regenerate(this.Tileset, 0, 0);
                    this.m_Regenerated = true;
                    Log.WriteLine("Generation complete.");
                }
            }
            else if (keystate.IsKeyUp(Keys.R))
                this.m_Regenerated = false;
            /*
            if (keystate.IsKeyDown(Keys.Add))
                this.RenderDepthValue += 1;
            else if (keystate.IsKeyDown(Keys.Subtract))
                this.RenderDepthValue -= 1;
            if (this.RenderDepthValue == 255)
                this.RenderDepthValue = 63;
            if (this.RenderDepthValue > 63)
                this.RenderDepthValue = 0;*/
            this.RenderDepthValue = (byte)(this.m_Player.WorldZ);
            this.RenderDepthUpRange = 8;
            this.RenderDepthDownRange = 8;
#endif

            return true; // update entities
        }
    }
}
