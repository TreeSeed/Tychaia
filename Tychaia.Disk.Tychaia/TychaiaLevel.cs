// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 

#if FALSE
namespace Tychaia.Disk.Tychaia
{
    public class TychaiaLevel : ILevel
    {
        // Store the octree in a file.  Basically you have a header for each
        // node that is just:
        //
        // 8 x <64-bit integer>
        //
        // with the integers indicating the position in the file for either
        // the next header or the actual file data, depending on the depth.
        //
        // TODO: Solve the issue of when chunks need a "larger" space (i.e
        // when they're storing temporary or dynamic data).  We could do this
        // by just storing the terrain data in the octree and putting entities
        // in a seperate file with a different structure.

        private string m_Path = null;
        private TerrainOctree m_Terrain = null;

        public TychaiaLevel(string name)
        {
            // Calculate path to level.
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.m_Path = Path.Combine(appdata, ".tychaia", "saves", name);
        }

        public void Save()
        {
        }

        public bool HasRegion(long x, long y, long z, long width, long height, long depth)
        {
            // FIXME: Don't rely on this.
            if (width != ChunkSize.Width || height != ChunkSize.Height || depth != ChunkSize.Depth)
                return false;

            // See if terrain.oct exists.
            if (!File.Exists(Path.Combine(this.m_Path, "terrain.oct")))
                return false;

            // Open the terrain octree.
            if (this.m_Terrain == null)
                this.m_Terrain = new TerrainOctree(Path.Combine(this.m_Path, "terrain.oct"));

            return this.m_Terrain.GetChunk(x / ChunkSize.Width, y / ChunkSize.Height, z / ChunkSize.Depth) != null;
        }

        public int[] ProvideRegion(long x, long y, long z, long width, long height, long depth)
        {
            // FIXME: Don't rely on this.
            if (width != ChunkSize.Width || height != ChunkSize.Height || depth != ChunkSize.Depth)
                return null;

            // See if terrain.oct exists.
            if (!File.Exists(Path.Combine(this.m_Path, "terrain.oct")))
                return null;

            // Open the terrain octree.
            if (this.m_Terrain == null)
                this.m_Terrain = new TerrainOctree(Path.Combine(this.m_Path, "terrain.oct"));

            // Open NBT.
            //NbtFile file = this.m_Terrain.GetChunk(x / ChunkSize.Width, y / ChunkSize.Height, z / ChunkSize.Depth);
            //if (file == null)
                return null;

            // Get the integer array for this chunk.
            //return file.RootTag["RawData"].IntArrayValue;
        }
    }
}
#endif