// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public interface IPredeterminedChunkPositions
    {
        IEnumerable<Vector3> GetRelativePositions();
        IEnumerable<Vector3> GetScaledRelativePositions();
        IEnumerable<Vector3> GetAbsolutePositions(Vector3 absolute);
        IEnumerable<RuntimeChunk> GetChunks(ChunkOctree octree, Vector3 focus);

        IEnumerable<Vector3> GetPurgableRelativePositions();
        IEnumerable<Vector3> GetPurgableScaledRelativePositions();
        IEnumerable<Vector3> GetPurgableAbsolutePositions(Vector3 absolute);
    }
}

