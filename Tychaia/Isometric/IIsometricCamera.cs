// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Microsoft.Xna.Framework;

namespace Tychaia
{
    public interface IIsometricCamera
    {
        int ChunkCenterX { get; set; }
        int ChunkCenterY { get; set; }
        Chunk Chunk { get; }
        ChunkOctree ChunkOctree { get; }

        void Pan(long x, long y, long z);
        void Focus(long x, long y, long z);
    }
}