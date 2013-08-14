// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia
{
    public interface IChunkSizePolicy
    {
        /// <summary>
        /// In 3D space, how many 3D voxels wide is a single cell in a chunk.
        /// </summary>
        int CellVoxelWidth { get; }

        /// <summary>
        /// In 3D space, how many 3D voxels high is a single cell in a chunk.
        /// </summary>
        int CellVoxelHeight { get; }

        /// <summary>
        /// In 3D space, how many 3D voxels deep is a single cell in a chunk.
        /// </summary>
        int CellVoxelDepth { get; }

        /// <summary>
        /// How many cells wide is a single chunk.
        /// </summary>
        int ChunkCellWidth { get; }

        /// <summary>
        /// How many cells high is a single chunk.
        /// </summary>
        int ChunkCellHeight { get; }

        /// <summary>
        /// How many cells deep is a single chunk.
        /// </summary>
        int ChunkCellDepth { get; }
    }
}
