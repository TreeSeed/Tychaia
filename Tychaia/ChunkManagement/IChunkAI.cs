// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;

namespace Tychaia
{
    public interface IChunkAI
    {
        /// <summary>
        /// Process the chunk AI.  Returns a list of chunks that should be rendered, or null
        /// if the render list shouldn't be changed.  There should only be one chunk AI that
        /// returns a non-null value from this function.
        /// </summary>
        Chunk[] Process(
            TychaiaGameWorld world,
            ChunkManagerEntity manager,
            IGameContext gameContext,
            IRenderContext renderContext);
    }
}

