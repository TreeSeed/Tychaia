using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tychaia.UI;
using Tychaia.Disk;

namespace Tychaia.Title
{
    public class TitleWorld : MenuWorld
    {
        private static bool m_GameJustStarted = true;
        private float m_FadeAmount = 0.0f;

        public TitleWorld()
        {
            if (m_GameJustStarted)
            {
                this.m_FadeAmount = 1.0f;
            }

            this.AddMenuItem("Generate World", () =>
            {
                this.m_TargetWorld = new RPGWorld(LevelAPI.NewLevel("Tychaia Demo World"));
            });
            this.AddMenuItem("Load Existing World", () =>
            {
                this.m_TargetWorld = new LoadWorld();
            });
            this.AddMenuItem("Randomize Seed", () =>
            {
                m_StaticSeed = m_Random.Next();
            });
            this.AddMenuItem("Exit", () =>
            {
                (this.Game as RuntimeGame).Exit();
            });
        }

        public override void DrawBelow(GameContext context)
        {
            if (this.m_FadeAmount < 1.0f)
                base.DrawBelow(context);
        }

        public override void DrawAbove(GameContext context)
        {
            if (this.m_FadeAmount < 1.0f)
                base.DrawAbove(context);

            var graphics = new XnaGraphics(context);
            graphics.FillRectangle(
                context.ScreenBounds,
                new Color(0, 0, 0, this.m_FadeAmount));
            this.m_FadeAmount -= 0.01f;
        }
    }
}
