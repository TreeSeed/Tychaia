// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;

namespace Tychaia.Runtime
{
    public interface IChunkCompressor
    {
        byte[] Compress(Chunk chunk);

        Chunk Decompress(byte[] bytes);
    }
}