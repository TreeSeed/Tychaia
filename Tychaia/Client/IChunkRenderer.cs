// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Protogame;
using Tychaia.Runtime;

namespace Tychaia.Client
{
    public interface IChunkRenderer
    {
        /// <summary>
        /// Renders the specified chunk with the specified rendering context.
        /// </summary>
        /// <param name="renderContext">The rendering context.</param>
        /// <param name="runtimeChunk">The runtime chunk to render.</param>
        void Render(IRenderContext renderContext, RuntimeChunk runtimeChunk);
    }
}