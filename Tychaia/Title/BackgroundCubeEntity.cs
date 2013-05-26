using System;
using Protogame;
using Tychaia.Generators;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class BackgroundCubeEntity : Entity
    {
        private int m_Distance;
        private GameContext m_Context;
        private static Random m_Random = new Random();
        private double m_ScreenX;
        private double m_ScreenY;

        public BackgroundCubeEntity(GameContext context, bool atTop = false)
        {
            this.m_ScreenX = m_Random.NextDouble();
            this.m_ScreenY = atTop ? 1.0 : m_Random.NextDouble();
            this.m_Distance = m_Random.Next(1, 50);
            this.m_Context = context;
        }

        public override void Update(World world)
        {
            this.m_ScreenY -= (100.0f / m_Distance) / 5000.0f;
            this.X = (float)(this.m_ScreenX * this.m_Context.ScreenBounds.Width);
            this.Y = (float)(this.m_ScreenY * this.m_Context.ScreenBounds.Height);

            if ((int)this.Y + (int)(TileIsometricifier.TILE_TOP_HEIGHT / this.m_Distance) +
                (int)(TileIsometricifier.TILE_SIDE_HEIGHT * 2.0 / this.m_Distance) < 0)
                world.Entities.Remove(this);

            base.Update(world);
        }

        public override void Draw(World world, XnaGraphics graphics)
        {
            graphics.DrawSprite(
                (int)this.X,
                (int)this.Y,
                (int)(TileIsometricifier.TILE_TOP_WIDTH * 2.0 / this.m_Distance),
                (int)(TileIsometricifier.TILE_TOP_HEIGHT * 2.0 / this.m_Distance),
                "tiles.grass.isometric.top",
                new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
            graphics.DrawSprite(
                (int)this.X + (int)(TileIsometricifier.TILE_SIDE_WIDTH * 2.0 / this.m_Distance),
                (int)this.Y + (int)(TileIsometricifier.TILE_TOP_HEIGHT / this.m_Distance),
                (int)(TileIsometricifier.TILE_SIDE_WIDTH * 2.0 / this.m_Distance),
                (int)(TileIsometricifier.TILE_SIDE_HEIGHT * 2.0 / this.m_Distance),
                "tiles.sand.isometric.sideL",
                new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
            graphics.DrawSprite(
                (int)this.X,
                (int)this.Y + (int)(TileIsometricifier.TILE_TOP_HEIGHT / this.m_Distance),
                (int)(TileIsometricifier.TILE_SIDE_WIDTH * 2.0 / this.m_Distance),
                (int)(TileIsometricifier.TILE_SIDE_HEIGHT * 2.0 / this.m_Distance),
                "tiles.sand.isometric.sideR",
                new Color(1.0f, 1.0f, 1.0f, 1.0f - (m_Distance / 100.0f)));
        }
    }
}

