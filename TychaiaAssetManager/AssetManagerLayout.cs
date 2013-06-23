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
        public Label Assets { get; private set; }
        public Button MarkDirty { get; private set; }

        public AssetManagerLayout()
        {
            var verticalContainer = new VerticalContainer();
            verticalContainer.AddChild(Status = new Label { Text = "Current Status" }, "40");
            verticalContainer.AddChild(MarkDirty = new Button { Text = "Mark Dirty" }, "40");
            verticalContainer.AddChild(Assets = new Label { Text = "Assets" }, "50%");
            verticalContainer.AddChild(new Button { Text = "Second" }, "*");
            verticalContainer.AddChild(new Label { Text = "25%" }, "25%");
            this.SetChild(verticalContainer);
        }
    }
}

