// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public class InventoryUIEntity : CanvasEntity
    {
        private HorizontalContainer m_SplitHorizontal;
        private VerticalContainer m_LeftContainer;
        private VerticalContainer m_CentreContainer;
        private VerticalContainer m_RightContainer;
        private HorizontalContainer m_StatusBarSpacing;
        private StatusBar m_StatusBar;

        public InventoryUIEntity(
            IGameUIFactory gameUIFactory,
            ISkin skin) : base(skin)
        {
            this.m_SplitHorizontal = new HorizontalContainer();
            this.m_LeftContainer = new VerticalContainer();
            this.m_CentreContainer = new VerticalContainer();
            this.m_RightContainer = new VerticalContainer();
            this.m_StatusBarSpacing = new HorizontalContainer();
            this.m_StatusBar = gameUIFactory.CreateStatusBar();

            this.m_SplitHorizontal.AddChild(this.m_LeftContainer, "0");
            this.m_SplitHorizontal.AddChild(this.m_CentreContainer, "*");
            this.m_SplitHorizontal.AddChild(this.m_RightContainer, "0");
            this.m_StatusBarSpacing.AddChild(new EmptyContainer(), "*");
            this.m_StatusBarSpacing.AddChild(this.m_StatusBar, "640");
            this.m_StatusBarSpacing.AddChild(new EmptyContainer(), "*");
            this.m_CentreContainer.AddChild(new EmptyContainer(), "*");
            this.m_CentreContainer.AddChild(this.m_StatusBarSpacing, "64");

            this.Canvas = new Canvas();
            this.Canvas.SetChild(this.m_SplitHorizontal);
        }
    }
}

