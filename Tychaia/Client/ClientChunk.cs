// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Data;
using Tychaia.Runtime;

namespace Tychaia
{
    public class ClientChunk : IChunk
    {
        public ClientChunk(long x, long y, long z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Cell[] Cells { get; set; }

        public bool Generated { get; set; }

        public int[] GeneratedIndices { get; set; }

        public VertexPositionTexture[] GeneratedVertexes { get; set; }

        public bool GraphicsEmpty { get; set; }

        public IndexBuffer IndexBuffer { get; set; }

        public VertexBuffer VertexBuffer { get; set; }

        public long X { get; private set; }

        public long Y { get; private set; }

        public long Z { get; private set; }
    }
}