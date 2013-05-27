using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if FALSE
namespace Tychaia.Disk.Tychaia
{
    public class TerrainOctree
    {
        private BinaryReader m_Reader;
        private StreamReader m_Stream;

        public TerrainOctree(string path)
        {
            this.m_Stream = new StreamReader(path);
            this.m_Reader = new BinaryReader(this.m_Stream.BaseStream);
        }

        private long GetMaskAtDepth(int maximal, int current)
        {
            return 0x1L << (maximal - current - 1);
        }

        public NbtFile GetChunk(long x, long y, long z)
        {
            int current = 0;
            int maximal = 64;
            long currentPos = 0;
            long length = 0;
            while ((currentPos != 0 || current == 0) && current != maximal)
            {
                // Read header data from file.
                this.m_Reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                long[] nodes = new long[8]
                {
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64()
                };
                long[] lengths = new long[8]
                {
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64(),
                    this.m_Reader.ReadInt64()
                };

                // Switch position based on octree.
                if ((x & this.GetMaskAtDepth(maximal, current)) == 0)
                {
                    if ((y & this.GetMaskAtDepth(maximal, current)) == 0)
                    {
                        if ((z & this.GetMaskAtDepth(maximal, current)) == 0)
                        {
                            currentPos = nodes[0];
                            length = lengths[0];
                        }
                        else
                        {
                            currentPos = nodes[1];
                            length = lengths[1];
                        }
                    }
                    else
                    {
                        if ((z & this.GetMaskAtDepth(maximal, current)) == 0)
                        {
                            currentPos = nodes[2];
                            length = lengths[2];
                        }
                        else
                        {
                            currentPos = nodes[3];
                            length = lengths[3];
                        }
                    }
                }
                else
                {
                    if ((y & this.GetMaskAtDepth(maximal, current)) == 0)
                    {
                        if ((z & this.GetMaskAtDepth(maximal, current)) == 0)
                        {
                            currentPos = nodes[4];
                            length = lengths[4];
                        }
                        else
                        {
                            currentPos = nodes[5];
                            length = lengths[5];
                        }
                    }
                    else
                    {
                        if ((z & this.GetMaskAtDepth(maximal, current)) == 0)
                        {
                            currentPos = nodes[6];
                            length = lengths[6];
                        }
                        else
                        {
                            currentPos = nodes[7];
                            length = lengths[7];
                        }
                    }
                }
            }

            // If currentPos is not equal to 0, it now contains the position of the
            // chunk data.
            if (currentPos != 0)
            {
                this.m_Reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                if (length >= Int32.MaxValue)
                    throw new InvalidOperationException("Chunk data can not be greater than 4GB.");
                byte[] data = this.m_Reader.ReadBytes((int)length);
                MemoryStream mem = new MemoryStream(data);
                return new NbtFile(mem, NbtCompression.ZLib);
            }
            else
                return null;
        }
    }
}
#endif
