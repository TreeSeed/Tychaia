// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using Substrate.Core;

namespace Tychaia
{
    public class RegionFile : IDisposable
    {
        private BinaryReader m_Reader;
        private StreamReader m_Stream;

        public RegionFile(string path)
        {
            this.m_Stream = new StreamReader(path);
            this.m_Reader = new BinaryReader(this.m_Stream.BaseStream);
        }

        public NBTFile GetChunk(long x, long z, long y)
        {
            long seek = 4 * (x % 32) + (z % 32) * 32;
            this.m_Reader.BaseStream.Seek(seek, SeekOrigin.Begin);
            byte[] offsetBytes = new byte[4] { 0, 0, 0, 0 };
            byte[] countBytes = new byte[1] { 0 };
            this.m_Reader.BaseStream.Read(offsetBytes, 1, 3);
            this.m_Reader.BaseStream.Read(countBytes, 0, 1);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(offsetBytes);
                Array.Reverse(countBytes);
            }
            int offset = BitConverter.ToInt32(offsetBytes, 0) * 4096;
            int count = countBytes[0] * 4096;
            this.m_Reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            byte[] lengthBytes = new byte[4];
            byte[] compressionBytes = new byte[1];
            this.m_Reader.BaseStream.Read(lengthBytes, 0, 4);
            this.m_Reader.BaseStream.Read(compressionBytes, 0, 1);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
                Array.Reverse(compressionBytes);
            }
            int length = BitConverter.ToInt32(lengthBytes, 0) * 4096;
            int compression = compressionBytes[0] * 4096;
            NbtCompression nbtComp = NbtCompression.AutoDetect;
            /*switch (compression)
            {
                case 0:
                    nbtComp = NbtCompression.None;
                    break;
                case 1:
                    nbtComp = NbtCompression.GZip;
                    break;
                case 2:
                    nbtComp = NbtCompression.ZLib;
                    break;
            }*/
            if (length == 0)
                return null;
            byte[] data = new byte[length];
            MemoryStream mem = new MemoryStream(data);
            this.m_Reader.BaseStream.Read(data, 0, length);
            return new NbtFile(mem, nbtComp);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.m_Stream.Dispose();
            this.m_Reader.Dispose();
        }

        #endregion
    }
}
