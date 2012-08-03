using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Tychaia.Generators;
using Microsoft.Xna.Framework.Graphics;

namespace Tychaia
{
    public class IsometricWorldManager : WorldManager
    {
        protected override void DrawTilesBelow(GameContext context)
        {
            if (!(context.World is RPGWorld))
                return;
            ChunkManager cm = (context.World as RPGWorld).ChunkManager;
            if (cm == null)
                return;

            Chunk c = null;
            Chunk cl = cm.ZerothChunk;
            int gx = -TileIsometricifier.CHUNK_TOP_WIDTH / 2 + TileIsometricifier.CHUNK_TOP_WIDTH / 2 * 3 - 60;
            int gy = -TileIsometricifier.CHUNK_TOP_HEIGHT / 2 * 4 - 90;
            int x = gx;
            int y = gy;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (c == null)
                        c = cl;
                    else
                        c = c.Right;
                    Texture2D tex = c.Texture;
                    if (tex != null)
                        context.SpriteBatch.Draw(tex, new Vector2(x, y), Color.White);
                    x += TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                    y += TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                }
                x = gx - (i + 1) * TileIsometricifier.CHUNK_TOP_WIDTH / 2;
                y = gy + (i + 1) * TileIsometricifier.CHUNK_TOP_HEIGHT / 2;
                cl = cl.Down;
                c = null;
            }
        }

        protected override void DrawTilesAbove(GameContext context)
        {
        }
    }
}
