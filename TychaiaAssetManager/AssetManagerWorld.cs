//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Assets;
using Microsoft.Xna.Framework.Input;
using System.Linq;

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

            var i = 0;
            foreach (var asset in this.AssetManager.GetAll().Cast<NetworkAsset>())
            {
                var dirtyMark = asset.Dirty ? "*" : "";
                xna.DrawStringLeft(20, 40 + i, asset.Name + dirtyMark);
                i += 16;
            }
            i += 16;
            xna.DrawStringLeft(20, 40 + i, "Assets marked with * are dirty; click to dirty all items.");
        }

        public override bool Update(GameContext context)
        {
            var newStatus = "Connected for " +
                (int)((DateTime.Now - this.m_Start).TotalSeconds) +
                " seconds";
            if (this.AssetManager.Status != newStatus)
                this.AssetManager.Status = newStatus;

            var state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed)
            {
                foreach (var asset in this.AssetManager.GetAll())
                    this.AssetManager.Dirty(asset.Name);
            }

            return true;
        }
    }
}
