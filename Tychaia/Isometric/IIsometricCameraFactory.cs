// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Runtime;

namespace Tychaia
{
    public interface IIsometricCameraFactory
    {
        IsometricCamera CreateIsometricCamera(ChunkOctree octree, RuntimeChunk chunk);
    }
}
