// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    public interface IAlgorithmFlowImageGeneration
    {
        Bitmap RegenerateImageForLayer(
            StorageLayer layer,
            long seed,
            long ox, long oy, long oz,
            int width, int height, int depth,
            bool compiled = false);
    }
}

