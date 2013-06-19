using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject;
using Protogame;
using Tychaia.Assets;
using Tychaia.Globals;
using Tychaia.UI;

namespace Tychaia.Title
{
    public abstract class MenuWorld : World
    {
        private List<TitleButton> m_Buttons = new List<TitleButton>();
        protected static Random m_Random = new Random();
        public static int m_StaticSeed = 6294563;
        protected World m_TargetWorld = null;
        private int m_PreviousX = 800;
        private int m_MenuItemY = 300;
        private ScatterBackground m_ScatterBackground;
        private IAssetManager m_AssetManager = null;

        protected MenuWorld()
        {
            var provider = IoC.Kernel.TryGet<IAssetManagerProvider>();
            if (provider != null)
                this.m_AssetManager = provider.GetAssetManager(false);
        }

        public void AddMenuItem(string name, Action handler)
        {
            this.m_Buttons.Add(new TitleButton(name, new Rectangle(this.m_PreviousX - 100, this.m_MenuItemY, 200, 30), handler));
            this.m_MenuItemY += 40;
        }

        private void AdjustButtons(GameContext context)
        {
            // Calculate the difference between button positions.
            int cx = context.Window.ClientBounds.Width / 2;
            int i = context.ScreenBounds.Height / 4 * 3;
            foreach (TitleButton b in this.m_Buttons)
            {
                b.X += cx - this.m_PreviousX;
                b.Y = i;
                i += 40;
            }
            this.m_PreviousX = cx;
        }

        public override bool Update(GameContext context)
        {
            if (this.m_ScatterBackground == null && context.FrameCount > 60)
                this.m_ScatterBackground = new ScatterBackground(context, this);
            if (this.m_ScatterBackground != null)
                this.m_ScatterBackground.Update(context, this);

            this.AdjustButtons(context);
            if (this.m_TargetWorld != null)
            {
                (this.Game as RuntimeGame).SwitchWorld(this.m_TargetWorld);
                return false;
            }
            return true;
        }

        public override void DrawBelow(GameContext context)
        {
            this.AdjustButtons(context);

            // Draw background effects.
            XnaGraphics xna = new XnaGraphics(context);
            xna.FillRectangle(
                context.ScreenBounds,
                Color.Black);
        }

        public override void DrawAbove(GameContext context)
        {
            XnaGraphics xna = new XnaGraphics(context);
            xna.DrawStringCentered(context.ScreenBounds.Width / 2, 50, "Tychaia", "TitleFont");

            // TODO: Draw animation of player falling here.
            xna.DrawSprite(
                context.ScreenBounds.Width / 2,
                context.ScreenBounds.Height / 2,
                "chars.player.player");

            MouseState state = Mouse.GetState();
            foreach (TitleButton b in this.m_Buttons)
                b.Process(xna, state);

            if (this.m_AssetManager != null)
            {
                xna.DrawStringCentered(
                context.ScreenBounds.Width / 2,
                10,
                "Asset Manager: " + this.m_AssetManager.Status,
                "Arial");
            }

            /*xna.DrawStringCentered(
                context.ScreenBounds.Width / 2,
                context.ScreenBounds.Height - 50,
                "Using static seed: " + m_StaticSeed.ToString(),
                "Arial");*/

            // Draw debug information.
            //DebugTracker.Draw(context, null);
        }
    }
}
