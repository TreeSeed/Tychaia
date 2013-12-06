// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public interface IIsometricBitmapRenderer
    {
        Bitmap GenerateImage(
            IGenerator generator,
            Func<dynamic, Color> getColor,
            long x, 
            long y, 
            long z,
            int width, 
            int height, 
            int depth);

        Bitmap GenerateImage(
            dynamic data,
            Func<dynamic, Color> getColor,
            int width, 
            int height, 
            int depth);
    }
}
