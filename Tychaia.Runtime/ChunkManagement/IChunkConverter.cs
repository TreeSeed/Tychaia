// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.Data;

namespace Tychaia.Runtime
{
    public interface IChunkConverter
    {
        void FromChunk<T>(Chunk chunk, T target) where T : IChunk;

        Chunk ToChunk<T>(T chunk) where T : IChunk;
    }
}