// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;

namespace Tychaia
{
    public interface ILevel
    {
        bool HasChunk(RuntimeChunk chunk);
        bool HasChunk(long x, long y, long z);
        void LoadChunk(RuntimeChunk chunk);
        void SaveChunk(RuntimeChunk chunk);
        void SaveChunk(long x, long y, long z, Cell[,,] data);
        void ScanChunks();
    }
}
