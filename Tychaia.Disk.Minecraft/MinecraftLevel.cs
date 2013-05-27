using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tychaia.Globals;

#if FALSE
namespace Tychaia.Disk.Minecraft
{
    public class MinecraftLevel : ILevel
    {
        private string m_Path = null;

        public MinecraftLevel(string name)
        {
            // Calculate path to level.
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.m_Path = Path.Combine(appdata, ".minecraft", "saves", name);
        }

        public void Save()
        {
            // Can't save the Minecraft format.
        }

        public const int MINECRAFT_CHUNK_WIDTH = 16;
        public const int MINECRAFT_CHUNK_HEIGHT = 256;
        public const int MINECRAFT_CHUNK_DEPTH = 16;
        public const int TYCHAIA_SCALE_WIDTH = MINECRAFT_CHUNK_WIDTH / ChunkSize.Width;
        public const int TYCHAIA_SCALE_HEIGHT = MINECRAFT_CHUNK_HEIGHT / ChunkSize.Height;
        public const int TYCHAIA_SCALE_DEPTH = MINECRAFT_CHUNK_DEPTH / ChunkSize.Depth;

        public bool HasRegion(long x, long z, long y, long width, long depth, long height)
        {
            long localX = x / TYCHAIA_SCALE_WIDTH >> 5;
            long localZ = z / TYCHAIA_SCALE_DEPTH >> 5;
            string regionPath = Path.Combine(this.m_Path, "region", "r." + localX + "." + localZ + ".mca");
            return File.Exists(regionPath);
        }

        public int[] ProvideRegion(long x, long z, long y, long width, long depth, long height)
        {
            int[] data = new int[width * height * depth];

            // Break the region we need to provide up into the Minecraft chunks.
            for (long i = x; i < width; i++)
                for (long j = z; j < depth; j++)
                    for (long k = y; k < height; k++)
                    {
                        long rx = i - x;
                        long rz = j - z;
                        long ry = k - y;

                        try
                        {
                            // Determine region.
                            long localX = i / TYCHAIA_SCALE_WIDTH >> 5;
                            long localZ = j / TYCHAIA_SCALE_DEPTH >> 5;
                            string regionPath = Path.Combine(this.m_Path, "region", "r." + localX + "." + localZ + ".mca");
                            if (!File.Exists(regionPath))
                            {
                                data[rx + rz * width + ry * width * depth] = 0;
                                continue;
                            }

                            // Load region.
                            RegionFile region = new RegionFile(regionPath);

                            // Load NBT.
                            NbtFile nbt = region.GetChunk(i - localX * ChunkSize.Width, j - localZ * ChunkSize.Depth, k);
                            if (nbt == null)
                            {
                                data[rx + rz * width + ry * width * depth] = 0;
                                region.Dispose();
                                continue;
                            }

                            // Find block.
                            foreach (NbtTag t in (NbtList)nbt.RootTag["Level"]["Sections"])
                            {
                                if (t["Y"].ByteValue == j / MINECRAFT_CHUNK_HEIGHT / 16)
                                {
                                    // Read exact block ID.
                                    long pos = j * 16 * 16 + k * 16 + i;
                                    data[rx + rz * width + ry * width * depth] = t["Blocks"].ByteArrayValue[pos];
                                    break;
                                }
                            }

                            region.Dispose();
                        }
                        catch (Exception e)
                        {
                            data[rx + rz * width + ry * width * depth] = 0;
                        }
                    }

            return data;
        }
    }
}
#endif
