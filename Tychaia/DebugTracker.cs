using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public class DebugTracker
    {
        public static void Draw(GameContext context, RPGWorld world)
        {
            XnaGraphics g = new XnaGraphics(context);
            g.FillRectangle(new Rectangle(0, 0, 300, 200), Color.Black);
            g.DrawRectangle(new Rectangle(0, 0, 300, 200), Color.Gray);
            g.DrawStringLeft(20, 20, "Render targets used: " + RenderTargetFactory.RenderTargetsUsed);
            g.DrawStringLeft(20, 20 + 16, "Render targets memory: " + RenderTargetFactory.RenderTargetMemory / (1 * 1024 * 1024) + "MB");
            if (world != null)
            {
                TimeSpan ts = new TimeSpan((long)((RPGWorld.AUTOSAVE_LIMIT - world.m_AutoSave) / 60.0 * 10000000.0));
                g.DrawStringLeft(20, 20 + 32, "Autosave counter: " + ts.Minutes + "m" + ts.Seconds + "s");
            }
        }
    }
}
