// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Data;

namespace Tychaia.Runtime
{
    public interface IChunk
    {
        Cell[] Cells { get; set; }

        bool Generated { get; set; }

        int[] GeneratedIndices { get; set; }

        VertexPositionTexture[] GeneratedVertexes { get; set; }

        bool GraphicsEmpty { get; set; }

        IndexBuffer IndexBuffer { get; set; }

        VertexBuffer VertexBuffer { get; set; }

        long X { get; }

        long Y { get; }

        long Z { get; }
        
        byte[] CompressedData { get; set; }
    }
}