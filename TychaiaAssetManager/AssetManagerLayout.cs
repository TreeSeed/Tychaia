//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.UI;

namespace TychaiaAssetManager
{
    public class AssetManagerLayout : Canvas
    {
        public Label Status { get; private set; }
        public Button MarkDirty { get; private set; }
        public MainMenu MainMenu { get; private set; }
        public TreeView AssetTree { get; private set; }

        public AssetManagerLayout()
        {
            var assetContainer = new VerticalContainer();
            assetContainer.AddChild(this.MarkDirty = new Button { Text = "Mark Dirty" }, "20");

            var contentContainer = new HorizontalContainer();
            contentContainer.AddChild(this.AssetTree = new TreeView(), "50%");
            contentContainer.AddChild(assetContainer, "50%");

            var menuContainer = new VerticalContainer();
            menuContainer.AddChild(this.MainMenu = new MainMenu(), "24");
            menuContainer.AddChild(contentContainer, "*");
            menuContainer.AddChild(this.Status = new Label { Text = "..." }, "24");
            this.SetChild(menuContainer);

            var openLocalItem = new MenuItem { Text = "Open Local Folder" };
            var connectItem = new MenuItem { Text = "Connect to Game" };
            var assetManagerMenuItem = new MenuItem { Text = "Asset Manager" };
            assetManagerMenuItem.AddChild(openLocalItem);
            assetManagerMenuItem.AddChild(connectItem);
            this.MainMenu.AddChild(assetManagerMenuItem);

            this.MainMenu.AddChild(this.GenerateTestMenu());

            this.AssetTree.AddChild(new TreeItem { Text = "language.TEST" });
            this.AssetTree.AddChild(new TreeItem { Text = "language.ANOTHER" });
            this.AssetTree.AddChild(new TreeItem { Text = "image.player.walk" });
            this.AssetTree.AddChild(new TreeItem { Text = "image.player.run" });
            this.AssetTree.AddChild(new TreeItem { Text = "image.player.attack" });
            this.AssetTree.AddChild(new TreeItem { Text = "image.enemy.walk" });
            this.AssetTree.AddChild(new TreeItem { Text = "image.enemy.run" });
            this.AssetTree.AddChild(new TreeItem { Text = "image.enemy.attack" });
        }

        public MenuItem GenerateTestMenu(int level = 0, string state = "")
        {
            if (level > 3)
                return null;
            var menu = new MenuItem { Text = state == "" ? "Test Menu" : ("Level " + state) };
            for (var i = 0; i < 5; i++)
            {
                var item = this.GenerateTestMenu(level + 1, (state + ", " + (i + 1)).Trim(',', ' '));
                if (item != null)
                    menu.AddChild(item);
            }
            return menu;
        }
    }
}

