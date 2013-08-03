//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public interface IRelativeChunkRendering
    {
        IEnumerable<RelativeRenderInformation> GetRelativeRenderInformation(IGameContext context, Chunk center, Vector3 focus);
    }
}

