// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom
    }
    
    public enum Corner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    
    public interface ITychaiaSkin
    {
        void DrawUIBorder(IRenderContext context, Rectangle layout, Side side);
        void DrawUICorner(IRenderContext context, Rectangle layout, Corner corner, string button = null);
        void DrawUIBackground(IRenderContext context, Rectangle layout);
    }
}
