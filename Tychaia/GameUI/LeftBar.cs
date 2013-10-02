// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class LeftBar : IContainer
    {
        private I2DRenderUtilities m_2DRenderUtilities;

        public LeftBar(I2DRenderUtilities twodRenderUtilities)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
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
                Color.Yellow,
                filled: true);
        }
    }
}
