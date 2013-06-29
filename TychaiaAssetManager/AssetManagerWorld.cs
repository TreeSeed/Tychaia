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
using System.Collections.Generic;
using System.Reflection;

namespace TychaiaAssetManager
{
    public class AssetManagerWorld : World
    {
        public IAssetManager AssetManager { get; set; }
        private DateTime m_Start;
        private AssetManagerLayout m_Layout;
        private static Dictionary<Type, IAssetEditor> m_Editors;

        static AssetManagerWorld()
        {
            LoadEditorsForAssets();
        }

        public static void LoadEditorsForAssets()
        {
            m_Editors = new Dictionary<Type, IAssetEditor>();
            foreach (var mapping in
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IAssetEditor).IsAssignableFrom(type)
                where !type.IsInterface
                where !type.IsAbstract
                let a = Activator.CreateInstance(type) as IAssetEditor
                select new {
                    AssetType = a.GetAssetType(),
                    Editor = a })
            {
                m_Editors.Add(mapping.AssetType, mapping.Editor);
            }
        }

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

            this.m_Layout.AssetTree.SelectedItemChanged += (sender, e) =>
            {
                var item = this.m_Layout.AssetTree.SelectedItem as AssetTreeItem;
                if (item != null && m_Editors.ContainsKey(item.Asset.GetType()))
                {
                    var editor = m_Editors[item.Asset.GetType()];
                    editor.SetAsset(item.Asset);
                    editor.BuildLayout(this.m_Layout.EditorContainer);
                }
                else
                {
                    this.m_Layout.EditorContainer.SetChild(
                        new Label { Text = "No editor for this asset" });
                }
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

            // Get the new state, and the items in the tree.
            var assets = this.AssetManager.GetAll();
            var existing = this.m_Layout.AssetTree.Children.Cast<TreeItem>();

            // Find items we need to add.
            foreach (var @add in assets.Where(x => !existing.Where(y => y is AssetTreeItem)
                .Cast<AssetTreeItem>().Select(y => y.Asset).Contains(x)))
            {
                var dirtyMark = "";
                if (@add is NetworkAsset)
                    dirtyMark = (@add as NetworkAsset).Dirty ? "*" : "";
                this.m_Layout.AssetTree.AddChild(new AssetTreeItem
                {
                    Text = @add.Name + dirtyMark,
                    Asset = @add.Resolve<IAsset>() // resolve any NetworkAssets
                });
            }

            // Find items we need to remove.
            foreach (var @remove in existing.Where(x => x is AssetTreeItem)
                .Cast<AssetTreeItem>().Where(x => !assets.Contains(x.Asset)))
            {
                this.m_Layout.AssetTree.RemoveChild(@remove);
            }

            return true;
        }
    }
}
