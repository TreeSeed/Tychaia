//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Assets;

namespace TychaiaAssetManager
{
    public class AssetManagerWorld : World
    {
        public IAssetManager AssetManager { get; set; }
        private DateTime m_Start;

        public AssetManagerWorld()
        {
            this.m_Start = DateTime.Now;
        }

        public override void DrawBelow(GameContext context)
        {
            context.Graphics.GraphicsDevice.Clear(Color.Black);
        }

        public override void DrawAbove(GameContext context)
        {
            var xna = new XnaGraphics(context);
            xna.DrawStringCentered(
                context.ScreenBounds.Width / 2,
                10,
                this.AssetManager.Status,
                "Arial");
        }

        public override bool Update(GameContext context)
        {
            var newStatus = "Connected for " +
                (int)((DateTime.Now - this.m_Start).TotalSeconds) +
                " seconds";
            if (this.AssetManager.Status != newStatus)
                this.AssetManager.Status = newStatus;

            return true;
        }
    }
}
