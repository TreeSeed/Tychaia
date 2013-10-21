// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public interface IFrustumChunkCache
    {
        /// <summary>
        /// Sets the frustum scope for the frustum cache.  The view matrix
        /// component must be looking at 0, 0, 0 in order for the cache to
        /// work correctly.
        /// </summary>
        /// <param name="frustum">Frustum.</param>
        void SetFrustumScope(Matrix frustum);
        IEnumerable<Vector3> GetRelativePositions();
        IEnumerable<Vector3> GetAbsolutePositions(Vector3 currentFocus);
        IEnumerable<RuntimeChunk> GetChunks(ChunkOctree octree, Vector3 focus);
    }
}
