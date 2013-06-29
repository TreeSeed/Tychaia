//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using Protogame;
using System.Collections.Generic;

namespace Tychaia.UI
{
    public class MainMenu : MenuItem
    {
        private IEnumerable<KeyValuePair<MenuItem, Rectangle>> GetChildLocations(Rectangle layout)
        {
            var accumulated = 0;
            foreach (var child in m_Items)
            {
                yield return new KeyValuePair<MenuItem, Rectangle>(
                    child,
                    new Rectangle(
                        layout.X + accumulated + 10,
                        layout.Y,
                        child.TextWidth + 10,
                        layout.Height));
                accumulated += child.TextWidth + 10;
            }
        }

        public override void Update(Rectangle layout, ref bool stealFocus)
        {
            foreach (var kv in GetChildLocations(layout))
                kv.Key.Update(kv.Value, ref stealFocus);
        }

        public override void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawMainMenu(graphics, layout, this);
            foreach (var kv in GetChildLocations(layout))
            {
                kv.Key.Draw(graphics, skin, kv.Value);
            }
        }
    }
}

