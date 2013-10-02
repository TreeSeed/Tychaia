// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class RightBar : SingleContainer
    {
        private I2DRenderUtilities m_2DRenderUtilities;

        public RightBar(I2DRenderUtilities twodRenderUtilities)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
        }

        public override void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            if (!context.Is3DContext)
            {
                this.m_2DRenderUtilities.RenderRectangle(
                    context,
                    layout,
                    Color.Green,
                    filled: true);
            }

            base.Draw(context, skin, layout);
        }
    }
}
