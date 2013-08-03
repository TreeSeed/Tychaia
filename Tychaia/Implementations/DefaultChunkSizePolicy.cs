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
        
        public int ChunkTextureSideWidth { get { return 16 * 2; } }
        public int ChunkTextureSideHeight { get { return 48 * 2; } }
        public int ChunkTextureTopWidth { get { return 32 * 2; } }
        public int ChunkTextureTopHeight { get { return 24 * 2; } }
        public int ChunkCubeHeight { get { return this.ChunkTextureSideHeight / 2 - 16; } }
        
        public int ChunkTopWidth { get { return this.ChunkTextureTopWidth * this.ChunkCellWidth; } }
        public int ChunkTopHeight { get { return this.ChunkTextureTopHeight * this.ChunkCellHeight; } }
    }
}

