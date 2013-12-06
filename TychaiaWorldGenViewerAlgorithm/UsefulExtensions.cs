// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;

namespace TychaiaWorldGenViewerAlgorithm
{
    internal static class UsefulExtensions
    {
        internal static Point Apply(this Point p, float f)
        {
            return new Point((int)(p.X * f), (int)(p.Y * f));
        }

        internal static Rectangle Apply(this Rectangle p, float f)
        {
            return new Rectangle(
                (int)(p.X * f),
                (int)(p.Y * f),
                (int)(p.Width * f),
                (int)(p.Height * f));
        }
    }
}
