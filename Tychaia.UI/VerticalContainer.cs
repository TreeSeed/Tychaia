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
    public class VerticalContainer : FlowContainer
    {
        protected override int GetMaximumContainerSize(Rectangle layout)
        {
            return layout.Height;
        }

        protected override Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size)
        {
            return new Rectangle(
                layout.X,
                layout.Y + accumulated,
                layout.Width,
                size);
        }

        public override void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawVerticalContainer(graphics, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderBy(x => x.Key.Order))
                kv.Key.Draw(graphics, skin, kv.Value);
        }
    }
}
