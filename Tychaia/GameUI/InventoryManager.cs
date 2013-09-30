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
            
            if (layout.Width == 0)
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
            
            // Render equipped items.
            this.m_2DRenderUtilities.RenderText(
                context,
                new Vector2(layout.X, layout.Y) +
                    new Vector2(20, 4),
                "Equipped Items:",
                this.m_DefaultFont,
                textColor: Color.Black,
                renderShadow: false);
            
            var equipHeight = (int)(layout.Width * 0.75) - 40;
            
            // Render large slot.
            this.m_2DRenderUtilities.RenderRectangle(
                context,
                new Rectangle(
                    layout.X + 20,
                    layout.Y + 20,
                    layout.Width / 2 - 40,
                    equipHeight),
                this.Inventory.HeavySlotItem == null ? Color.Purple : Color.Red,
                filled: true);
            if (this.Inventory.HeavySlotItem != null)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(
                        layout.X + 20,
                        layout.Y + 20),
                    this.Inventory.HeavySlotItem.Name,
                    this.m_DefaultFont,
                    textColor: Color.White,
                    renderShadow: false,
                    verticalAlignment: VerticalAlignment.Center);
            }

            // Render medium slot 1.
            this.m_2DRenderUtilities.RenderRectangle(
                context,
                new Rectangle(
                    layout.X + layout.Width / 2,
                    layout.Y + 20,
                    (int)(layout.Width * 0.3) - 40,
                    equipHeight / 2 - 10),
                this.Inventory.MediumSlot1Item == null ? Color.Purple : Color.Red,
                filled: true);
            if (this.Inventory.MediumSlot1Item != null)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(
                        layout.X + layout.Width / 2,
                        layout.Y + 20),
                    this.Inventory.MediumSlot1Item.Name,
                    this.m_DefaultFont,
                    textColor: Color.White,
                    renderShadow: false,
                    verticalAlignment: VerticalAlignment.Center);
            }

            // Render medium slot 2.
            this.m_2DRenderUtilities.RenderRectangle(
                context,
                new Rectangle(
                    layout.X + layout.Width / 2,
                    layout.Y + equipHeight / 2 + 30,
                    (int)(layout.Width * 0.3) - 40,
                    equipHeight / 2 - 10),
                this.Inventory.MediumSlot2Item == null ? Color.Purple : Color.Red,
                filled: true);
            if (this.Inventory.MediumSlot2Item != null)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(
                        layout.X + layout.Width / 2,
                        layout.Y + equipHeight / 2 + 30),
                    this.Inventory.MediumSlot2Item.Name,
                    this.m_DefaultFont,
                    textColor: Color.White,
                    renderShadow: false,
                    verticalAlignment: VerticalAlignment.Center);
            }
                
            // Render light slot.
            this.m_2DRenderUtilities.RenderRectangle(
                context,
                new Rectangle(
                    layout.X + layout.Width / 2 + (int)(layout.Width * 0.3) - 20,
                    layout.Y + 20,
                    layout.Width - (layout.Width / 2 + (int)(layout.Width * 0.3) - 20) - 20,
                    layout.Width - (layout.Width / 2 + (int)(layout.Width * 0.3) - 20) - 20),
                this.Inventory.LightSlotItem == null ? Color.Purple : Color.Red,
                filled: true);
            if (this.Inventory.LightSlotItem != null)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(
                        layout.X + layout.Width / 2 + (int)(layout.Width * 0.3) - 20,
                        layout.Y + 20),
                    this.Inventory.LightSlotItem.Name,
                    this.m_DefaultFont,
                    textColor: Color.White,
                    renderShadow: false,
                    verticalAlignment: VerticalAlignment.Center);
            }
                
            // Render unequipped items.
            this.m_2DRenderUtilities.RenderText(
                context,
                new Vector2(layout.X, layout.Y) +
                    new Vector2(20, equipHeight + 40),
                "Unequipped Items:",
                this.m_DefaultFont,
                textColor: Color.Black,
                renderShadow: false);
                
            // TODO: Write a listbox UI control and use that instead; at the moment
            // this doesn't support scrolling so lots of items will just render off
            // screen.
            var i = 0;
            foreach (var item in this.Inventory.UnequippedItems)
            {
                this.m_2DRenderUtilities.RenderRectangle(
                    context,
                    new Rectangle(
                        layout.X + 20,
                        layout.Y + equipHeight + 60 + i * 24,
                        layout.Width - 40,
                        24),
                    Color.Purple,
                    filled: true);
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(layout.X, layout.Y) +
                        new Vector2(40, equipHeight + 60 + 12) + 
                        new Vector2(0, i * 24),
                    item.Name,
                    this.m_DefaultFont,
                    textColor: Color.White,
                    renderShadow: false,
                    verticalAlignment: VerticalAlignment.Center);
                i++;
            }
        }
    }
}

