// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;

namespace Tychaia.Runtime
{
    public interface ILevel
    {
        bool HasChunk(IChunk chunk);
        bool HasChunk(long x, long y, long z);
        void LoadChunk(IChunk chunk);
        void SaveChunk(IChunk chunk);
        void SaveChunk(long x, long y, long z, Cell[,,] data);
        void ScanChunks();
    }
}
