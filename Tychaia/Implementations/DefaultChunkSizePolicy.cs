//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Tychaia
{
    public class DefaultChunkSizePolicy : IChunkSizePolicy
    {
        public int CellVoxelWidth { get { return 32; } }
        public int CellVoxelHeight { get { return 32; } }
        public int CellVoxelDepth { get { return 32; } }
        public int ChunkCellWidth { get { return 8; } }
        public int ChunkCellHeight { get { return 8; } }
        public int ChunkCellDepth { get { return 8; } }
        
        public int CellTextureSidePixelWidth { get { return 16 * 2; } }
        public int CellTextureSidePixelHeight { get { return 48 * 2; } }
        public int CellTextureTopPixelWidth { get { return 32 * 2; } }
        public int CellTextureTopPixelHeight { get { return 24 * 2; } }
        public int CellCubePixelHeight { get { return this.CellTextureSidePixelHeight / 2 - 16; } }
        
        public int ChunkTopPixelWidth { get { return this.CellTextureTopPixelWidth * this.ChunkCellWidth; } }
        public int ChunkTopPixelHeight { get { return this.CellTextureTopPixelHeight * this.ChunkCellHeight; } }
    }
}

