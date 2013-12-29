// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Dx.Runtime;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Data;

namespace Tychaia.Runtime
{
    public class RuntimeChunk
    {
        [Synchronised]
        public readonly long X;

        [Synchronised]
        public readonly long Y;

        [Synchronised]
        public readonly long Z;

        public RuntimeChunk(long x, long y, long z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        [Synchronised]
        public Cell[] Cells { get; set; }

        [Synchronised]
        public bool Generated { get; set; }

        [Synchronised]
        public int[] GeneratedIndices { get; set; }

        [Synchronised]
        public VertexPositionTexture[] GeneratedVertexes { get; set; }

        public bool GraphicsEmpty { get; set; }

        public VertexBuffer VertexBuffer { get; set; }

        public IndexBuffer IndexBuffer { get; set; }
    }
}