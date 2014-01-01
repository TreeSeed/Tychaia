// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;

namespace Tychaia.Runtime
{
    public class ChunkGenerationRequest
    {
        public Action Callback { get; set; }

        public IChunk Chunk { get; set; }
    }
}