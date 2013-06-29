//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia.UI
{
    public interface ISkin
    {
        void DrawCanvas(XnaGraphics graphics, Rectangle layout, Canvas canvas);
        void DrawFixedContainer(XnaGraphics graphics, Rectangle layout, FixedContainer fixedContainer);
        void DrawButton(XnaGraphics graphics, Rectangle layout, Button button);
        void DrawLabel(XnaGraphics graphics, Rectangle layout, Label label);
        void DrawLink(XnaGraphics graphics, Rectangle layout, Link link);
        void DrawVerticalContainer(XnaGraphics graphics, Rectangle layout, VerticalContainer verticalContainer);
        void DrawHorizontalContainer(XnaGraphics graphics, Rectangle layout, HorizontalContainer horizontalContainer);
        void DrawMenuItem(XnaGraphics graphics, Rectangle layout, MenuItem menuItem);
        void DrawMenuList(XnaGraphics graphics, Rectangle layout, MenuItem menuItem);
        void DrawMainMenu(XnaGraphics graphics, Rectangle layout, MainMenu mainMenu);
        void DrawTreeView(XnaGraphics graphics, Rectangle layout, TreeView treeView);
        void DrawTreeItem(XnaGraphics graphics, Rectangle layout, TreeItem treeItem);

        int HeightForTreeItem { get; }
        int MainMenuHorizontalPadding { get; }
        int AdditionalMenuItemWidth { get; }
        int MenuItemHeight { get; }
    }
}

