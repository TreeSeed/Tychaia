//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Tychaia.Assets;
using Tychaia.UI;
using Tychaia.Globals;
using Ninject;

namespace TychaiaAssetManager
{
    public class AssetManagerWorld : World
    {
        public IAssetManager AssetManager { get; set; }
        private DateTime m_Start;
        private AssetManagerLayout m_Layout;

        public AssetManagerWorld()
        {
            this.m_Start = DateTime.Now;

            // Add the asset manager layout.
            this.Entities.Add(new CanvasEntity(
                IoC.Kernel.Get<ISkin>(),
                this.m_Layout = new AssetManagerLayout()));

            this.m_Layout.MarkDirty.Click += (sender, e) =>
            {
                foreach (var asset in this.AssetManager.GetAll())
                    this.AssetManager.Dirty(asset.Name);
            };
        }

        public override void DrawBelow(GameContext context)
        {
        }

        public override void DrawAbove(GameContext context)
        {
        /*
            this.m_Layout.Assets.Text = "";
            foreach (var asset in this.AssetManager.GetAll().Cast<NetworkAsset>())
            {
                var dirtyMark = asset.Dirty ? "*" : "";
                this.m_Layout.Assets.Text += asset.Name + dirtyMark + "\r\n";
            }
            this.m_Layout.Assets.Text +=
                "Assets marked with * are dirty; click button above to dirty all items.";
                */
        }

        public override bool Update(GameContext context)
        {
            var newStatus = "Connected for " +
                (int)((DateTime.Now - this.m_Start).TotalSeconds) +
                " seconds";
            if (this.AssetManager.Status != newStatus)
                this.AssetManager.Status = newStatus;
            this.m_Layout.Status.Text = this.AssetManager.Status;

            return true;
        }
    }
}
