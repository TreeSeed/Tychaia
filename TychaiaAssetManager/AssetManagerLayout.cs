//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.UI;
using System;

namespace TychaiaAssetManager
{
    public class AssetManagerLayout : Canvas
    {
        public Label Status { get; private set; }
        public Button MarkDirty { get; private set; }
        public MainMenu MainMenu { get; private set; }
        public TreeView AssetTree { get; private set; }
        public SingleContainer EditorContainer { get; private set; }

        public AssetManagerLayout()
        {
            var toolbarContainer = new HorizontalContainer();
            toolbarContainer.AddChild(new SingleContainer(), "*");
            toolbarContainer.AddChild(this.MarkDirty = new Button { Text = "Mark Dirty" }, "80");

            var assetContainer = new VerticalContainer();
            assetContainer.AddChild(toolbarContainer, "20");
            assetContainer.AddChild(this.EditorContainer = new SingleContainer(), "*");

            var contentContainer = new HorizontalContainer();
            contentContainer.AddChild(this.AssetTree = new TreeView(), "50%");
            contentContainer.AddChild(assetContainer, "50%");

            var menuContainer = new VerticalContainer();
            menuContainer.AddChild(this.MainMenu = new MainMenu(), "24");
            menuContainer.AddChild(contentContainer, "*");
            menuContainer.AddChild(this.Status = new Label { Text = "..." }, "24");
            this.SetChild(menuContainer);

            var exitItem = new MenuItem { Text = "Exit" };
            var assetManagerMenuItem = new MenuItem { Text = "Asset Manager" };
            exitItem.Click += (sender, e) =>
            {
                Environment.Exit(0);
            };
            assetManagerMenuItem.AddChild(exitItem);
            this.MainMenu.AddChild(assetManagerMenuItem);
        }
    }
}

