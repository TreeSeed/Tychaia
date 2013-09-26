// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class InventoryManager : IContainer
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        private readonly IAssetManager m_AssetManager;
        private readonly FontAsset m_DefaultFont;

        public InventoryManager(
            I2DRenderUtilities _2DRenderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_2DRenderUtilities = _2DRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
        }

        public IContainer[] Children
        {
            get
            {
                return new IContainer[0];
            }
        }

        public IContainer Parent
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }

        public bool Focused
        {
            get;
            set;
        }

        public Tychaia.Game.Inventory Inventory
        {
            get;
            set;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            if (context.Is3DContext)
                return;

            if (this.Inventory == null)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(layout.X, layout.Y),
                    "No inventory selected",
                    this.m_DefaultFont,
                    textColor: Color.Black,
                    renderShadow: false);
                return;
            }

            this.m_2DRenderUtilities.RenderText(
                context,
                new Vector2(layout.X, layout.Y),
                "All items in inventory:",
                this.m_DefaultFont,
                textColor: Color.Black,
                renderShadow: false);
            var i = 16;
            foreach (var item in this.Inventory.AllItems)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(layout.X, layout.Y + i),
                    item.Name,
                    this.m_DefaultFont,
                    textColor: Color.Black,
                    renderShadow: false);
                i += 16;
            }

            this.m_2DRenderUtilities.RenderText(
                context,
                new Vector2(layout.X, layout.Y + i),
                "Equipped items in inventory:",
                this.m_DefaultFont,
                textColor: Color.Black,
                renderShadow: false);
            i += 16;
            foreach (var item in this.Inventory.EquippedItems)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(layout.X, layout.Y + i),
                    item.Name,
                    this.m_DefaultFont,
                    textColor: Color.Black,
                    renderShadow: false);
                i += 16;
            }
        }
    }
}

