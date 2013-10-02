// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Globals
{
    public class DefaultPositionScaleTranslation : IPositionScaleTranslation
    {
        private readonly int m_ChunkVoxelWidth;

        public DefaultPositionScaleTranslation(
            IChunkSizePolicy chunkSizePolicy)
        {
            this.m_ChunkVoxelWidth = chunkSizePolicy.CellVoxelWidth * chunkSizePolicy.ChunkCellWidth;
        }

        public long Translate(long v)
        {
            if (v < 0 && v % this.m_ChunkVoxelWidth != 0)
                return (v / this.m_ChunkVoxelWidth) - 1;
            return v / this.m_ChunkVoxelWidth;
        }
    }
}
