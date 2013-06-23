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

        void Update(Rectangle layout);
        void Draw(XnaGraphics graphics, ISkin skin, Rectangle layout);
    }
}
