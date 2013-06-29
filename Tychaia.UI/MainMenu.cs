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
        private IEnumerable<KeyValuePair<MenuItem, Rectangle>> GetChildLocations(ISkin skin, Rectangle layout)
        {
            var accumulated = -skin.MainMenuHorizontalPadding;
            foreach (var child in m_Items)
            {
                yield return new KeyValuePair<MenuItem, Rectangle>(
                    child,
                    new Rectangle(
                        layout.X + accumulated + skin.MainMenuHorizontalPadding,
                        layout.Y,
                        child.TextWidth + skin.MainMenuHorizontalPadding,
                        layout.Height));
                accumulated += child.TextWidth + skin.MainMenuHorizontalPadding;
            }
        }

        public override void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in GetChildLocations(skin, layout))
                kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
        }

        public override void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout)
        {
            skin.DrawMainMenu(graphics, layout, this);
            foreach (var kv in GetChildLocations(skin, layout))
            {
                kv.Key.Draw(graphics, skin, kv.Value);
            }
        }
    }
}

