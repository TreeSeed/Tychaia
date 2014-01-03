// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Runtime;

namespace Tychaia
{
    public interface IIsometricCamera<T> where T : class, IChunk
    {
        T Chunk { get; }

        int ChunkCenterX { get; set; }

        int ChunkCenterY { get; set; }

        ChunkOctree<T> ChunkOctree { get; }

        int Distance { get; set; }

        bool Orthographic { get; set; }

        bool Rotation { get; set; }

        float VerticalAngle { get; set; }

        void Focus(long x, long y, long z);

        void Pan(long x, long y, long z);
    }
}