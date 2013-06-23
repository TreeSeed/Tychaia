//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia.UI
{
    public class FixedContainer : BaseContainer, IContainer
    {
        public Rectangle AbsoluteRectangle { get; set; }

        public FixedContainer(Rectangle absolute)
        {
            this.AbsoluteRectangle = absolute;
        }

        public void Update(Rectangle layout)
        {
            if (this.Child != null)
                this.Child.Update(this.AbsoluteRectangle);
        }

        public void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawFixedContainer(graphics, layout, this);
            if (this.Child != null)
                this.Child.Draw(graphics, skin, this.AbsoluteRectangle);
        }
    }
}

