// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using Protogame;

namespace Tychaia
{
    public class InventoryUIEntity : CanvasEntity, IEventTogglable
    {
        private readonly IViewportMode m_ViewportMode;

        private HorizontalContainer m_SplitHorizontal;
        private LeftBar m_LeftBar;
        private VerticalContainer m_CentreContainer;
        private RightBar m_RightBar;
        private HorizontalContainer m_StatusBarSpacing;
        private StatusBar m_StatusBar;
        private InventoryManager m_InventoryManager;

        public InventoryUIEntity(
            IGameUIFactory gameUIFactory,
            IViewportMode viewportMode,
            ISkin skin) : base(skin)
        {
            this.m_ViewportMode = viewportMode;
            this.SidebarWidth = 300;

            this.m_InventoryManager = gameUIFactory.CreateInventoryManager();

            this.m_SplitHorizontal = new HorizontalContainer();
            this.m_LeftBar = gameUIFactory.CreateLeftBar();
            this.m_CentreContainer = new VerticalContainer();
            this.m_RightBar = gameUIFactory.CreateRightBar();
            this.m_StatusBarSpacing = new HorizontalContainer();
            this.m_StatusBar = gameUIFactory.CreateStatusBar();

            this.m_RightBar.SetChild(this.m_InventoryManager);

            this.m_SplitHorizontal.AddChild(this.m_LeftBar, "0");
            this.m_SplitHorizontal.AddChild(this.m_CentreContainer, "*");
            this.m_SplitHorizontal.AddChild(this.m_RightBar, "0");
            this.m_StatusBarSpacing.AddChild(new EmptyContainer(), "*");
            this.m_StatusBarSpacing.AddChild(this.m_StatusBar, "640");
            this.m_StatusBarSpacing.AddChild(new EmptyContainer(), "*");
            this.m_CentreContainer.AddChild(new EmptyContainer(), "*");
            this.m_CentreContainer.AddChild(this.m_StatusBarSpacing, "64");

            this.Canvas = new Canvas();
            this.Canvas.SetChild(this.m_SplitHorizontal);
        }

        public int SidebarWidth { get; set; }
        public bool RightExtended { get; private set; }
        public bool LeftExtended { get; private set; }

        public void Toggle(string id)
        {
            switch (id)
            {
                case "inventory":
                    this.ToggleRight();
                    break;
                case "character":
                    this.ToggleLeft();
                    break;
            }
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);

            this.m_ViewportMode.SidebarWidth = this.SidebarWidth;
            if (this.RightExtended && this.LeftExtended)
                this.m_ViewportMode.SetViewportMode(ViewportMode.Centre);
            else if (this.RightExtended)
                this.m_ViewportMode.SetViewportMode(ViewportMode.Left);
            else if (this.LeftExtended)
                this.m_ViewportMode.SetViewportMode(ViewportMode.Right);
            else
                this.m_ViewportMode.SetViewportMode(ViewportMode.Full);

            // Assign player's data to the UI.
            var players = gameContext.World.Entities.OfType<PlayerEntity>().ToArray();
            if (players.Length > 0)
            {
                var player = players[0];
                this.m_InventoryManager.Inventory = player.RuntimeData.Inventory;
                this.m_StatusBar.Player = player.RuntimeData;
            }
        }

        private void ToggleRight()
        {
            this.RightExtended = !this.RightExtended;
            this.m_SplitHorizontal.SetChildSize(
                this.m_RightBar,
                this.RightExtended ? this.SidebarWidth.ToString() : "0");
        }

        private void ToggleLeft()
        {
            this.LeftExtended = !this.LeftExtended;
            this.m_SplitHorizontal.SetChildSize(
                this.m_LeftBar,
                this.LeftExtended ? this.SidebarWidth.ToString() : "0");
        }
    }
}
