// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Globals
{
    public class DefaultChunkSizePolicy : IChunkSizePolicy
    {
        public int CellVoxelWidth
        {
            get { return 32; }
        }

        public int CellVoxelHeight
        {
            get { return 32; }
        }

        public int CellVoxelDepth
        {
            get { return 32; }
        }

        public int ChunkCellWidth
        {
            get { return 32; }
        }

        public int ChunkCellHeight
        {
            get { return 32; }
        }

        public int ChunkCellDepth
        {
            get { return 32; }
        }
    }
}
