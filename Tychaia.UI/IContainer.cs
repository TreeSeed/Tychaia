//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia.UI
{
    public interface IContainer
    {
        IContainer[] Children { get; }
        IContainer Parent { get; set; }
        int Order { get; set; }
        bool Focused { get; set; }

        void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus);
        void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout);
    }
}
