// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Game;

namespace Tychaia
{
    public class StatusBar : IContainer
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        private readonly IAssetManager m_AssetManager;
        private readonly FontAsset m_DefaultFont;

        public StatusBar(
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
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
        
        public Being Player
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

            this.m_2DRenderUtilities.RenderRectangle(
                context,
                layout,
                Color.Black,
                filled: true);
            
            if (this.Player == null)
                return;
            
            this.RenderBar(
                context,
                layout,
                0,
                this.Player.MaxHealth,
                this.Player.Health,
                Color.Red,
                "No Health");
                
            this.RenderBar(
                context,
                layout,
                14,
                this.Player.MaxStamina,
                this.Player.Stamina,
                Color.Yellow,
                "No Stamina");
                
            this.RenderBar(
                context,
                layout,
                28,
                this.Player.MaxMana,
                this.Player.Mana,
                Color.Blue,
                "No Mana");
        }

        private void RenderBar(
            IRenderContext context,
            Rectangle layout,
            int y,
            int max,
            int current,
            Color barColor,
            string noMessage)
        {
            if (max == 0)
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(layout.X, layout.Y) + new Vector2(20, 10 + y),
                    noMessage,
                    this.m_DefaultFont,
                    textColor: Color.White,
                    renderShadow: false);
            }
            else
            {
                this.m_2DRenderUtilities.RenderRectangle(
                    context,
                    new Rectangle(
                        layout.X + 20,
                        layout.Y + 10 + y,
                        100,
                        10),
                    new Color(63, 63, 63),
                    filled: true);
                this.m_2DRenderUtilities.RenderRectangle(
                    context,
                    new Rectangle(
                        layout.X + 20,
                        layout.Y + 10 + y,
                        (int)(100.0 / max * current),
                        10),
                    barColor,
                    filled: true);
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(layout.X, layout.Y) + new Vector2(25 + (int)(100.0 / max * current), 9 + y),
                    current.ToString(),
                    this.m_DefaultFont,
                    textColor: Color.White,
                    renderShadow: false);
            }
        }
    }
}
