//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;
using System.Linq;

namespace Tychaia.UI
{
    public class HorizontalContainer : FlowContainer
    {
        protected override int GetMaximumContainerSize(Rectangle layout)
        {
            return layout.Width;
        }

        protected override Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size)
        {
            return new Rectangle(
                layout.X + accumulated,
                layout.Y,
                size,
                layout.Height);
        }

        public override void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawHorizontalContainer(graphics, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderByDescending(x => x.Key.Order))
                kv.Key.Draw(graphics, skin, kv.Value);
        }
    }
}

