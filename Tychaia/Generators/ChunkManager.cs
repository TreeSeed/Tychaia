using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Generators
{
    public class ChunkManager
    {
        public Chunk ZerothChunk
        {
            get;
            private set;
        }

        public ChunkManager()
        {
            ZerothChunk = new Chunk(0, 0);
        }
    }
}
