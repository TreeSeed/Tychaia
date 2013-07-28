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
        private IRenderUtilities m_RenderUtilities;
        
        public DebugTracker(IRenderUtilities renderUtilities)
        {
            this.m_RenderUtilities = renderUtilities;
        }
        
        public static void Draw(IRenderContext renderContext, RPGWorld world)
        {
            this.m_RenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, 300, 200),
                Color.Black,
                filled: true);
            this.m_RenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, 300, 200),
                Color.Gray);
            this.m_RenderUtilities.RenderText(
                renderContext,
                new Vector2(20, 20),
                "Render targets used: " + RenderTargetFactory.RenderTargetsUsed);
            this.m_RenderUtilities.RenderText(
                renderContext,
                new Vector2(20, 20 + 16),
                "Render targets memory: " + RenderTargetFactory.RenderTargetMemory / (1 * 1024 * 1024) + "MB");
            if (world != null)
            {
                TimeSpan ts = new TimeSpan((long)((RPGWorld.AUTOSAVE_LIMIT - world.m_AutoSave) / 60.0 * 10000000.0));
                this.m_RenderUtilities.RenderText(
                    renderContext,
                    new Vector2(20, 20 + 32),
                    "Autosave counter: " + ts.Minutes + "m" + ts.Seconds + "s");
            }
        }
    }
}
