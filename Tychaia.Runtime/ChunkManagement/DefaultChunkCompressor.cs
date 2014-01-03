// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Protogame;
using Protogame.Compression;
using Tychaia.Asset;
using Tychaia.Data;
using Tychaia.Globals;

namespace Tychaia.Runtime
{
    public class DefaultChunkCompressor : IChunkCompressor
    {
        private readonly IAssetManagerProvider m_AssetManagerProvider;

        private readonly Dictionary<int, string> m_BlocksByID;

        private readonly Dictionary<string, int> m_BlocksByName;

        private readonly int m_ChunkCellArrayLength;

        private readonly IChunkSizePolicy m_ChunkSizePolicy;

        public DefaultChunkCompressor(IAssetManagerProvider assetManagerProvider, IChunkSizePolicy chunkSizePolicy)
        {
            this.m_AssetManagerProvider = assetManagerProvider;
            this.m_ChunkSizePolicy = chunkSizePolicy;

            this.m_ChunkCellArrayLength = this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellHeight
                                          * this.m_ChunkSizePolicy.ChunkCellDepth;

            this.m_BlocksByName =
                this.m_AssetManagerProvider.GetAssetManager()
                    .GetAll()
                    .OfType<BlockAsset>()
                    .ToDictionary(key => key.Name, value => value.BlockID);

            this.m_BlocksByID =
                this.m_AssetManagerProvider.GetAssetManager()
                    .GetAll()
                    .OfType<BlockAsset>()
                    .ToDictionary(key => key.BlockID, value => value.Name);
        }

        public byte[] Compress(Chunk chunk)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(chunk.X);
                    writer.Write(chunk.Y);
                    writer.Write(chunk.Z);

                    if (chunk.Cells == null)
                    {
                        throw new InvalidOperationException();
                    }

                    for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                    {
                        for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                        {
                            var i = x + (z * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellDepth);
                            writer.Write(chunk.Cells[i].HeightMap);
                        }
                    }

                    for (var i = 0; i < this.m_ChunkCellArrayLength; i++)
                    {
                        var edgePosition = EdgePoint.Decompress(chunk.Cells[i].EdgePoint);

                        if (!edgePosition.RenderAbove && !edgePosition.RenderBelow && !edgePosition.RenderEast
                            && !edgePosition.RenderNorth && !edgePosition.RenderSouth && !edgePosition.RenderWest)
                        {
                            writer.Write(false);
                        }
                        else
                        {
                            writer.Write(true);
                            writer.Write(chunk.Cells[i].EdgePoint);
                            if (chunk.Cells[i].BlockAssetName == null)
                            {
                                writer.Write(byte.MaxValue);
                            }
                            else
                            {
                                writer.Write((byte)this.m_BlocksByName[chunk.Cells[i].BlockAssetName]);
                            }
                        }
                    }

                    var uncompressed = memory.Length;

                    using (var compressed = new MemoryStream())
                    {
                        memory.Seek(0, SeekOrigin.Begin);

                        var start = DateTime.Now;
                        LzmaHelper.Compress(memory, compressed);

                        Console.WriteLine(
                            "uncompressed " + uncompressed + " / compressed " + compressed.Position + " in "
                            + (DateTime.Now - start).TotalMilliseconds + "ms");

                        var length = compressed.Position;
                        var bytes = new byte[length];
                        compressed.Seek(0, SeekOrigin.Begin);
                        compressed.Read(bytes, 0, (int)length);
                        return bytes;
                    }
                }
            }
        }

        public Chunk Decompress(byte[] bytes)
        {
            var chunk = new Chunk { Cells = new Cell[this.m_ChunkCellArrayLength] };

            using (var compressed = new MemoryStream(bytes))
            {
                using (var memory = new MemoryStream())
                {
                    LzmaHelper.Decompress(compressed, memory);
                    memory.Seek(0, SeekOrigin.Begin);

                    using (var reader = new BinaryReader(memory))
                    {
                        chunk.X = reader.ReadInt64();
                        chunk.Y = reader.ReadInt64();
                        chunk.Z = reader.ReadInt64();

                        var lookup =
                            new int[this.m_ChunkSizePolicy.ChunkCellWidth, this.m_ChunkSizePolicy.ChunkCellDepth];
                        for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                        {
                            for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                            {
                                lookup[x, z] = reader.ReadInt32();
                            }
                        }

                        for (var i = 0; i < this.m_ChunkCellArrayLength; i++)
                        {
                            var x = i % this.m_ChunkSizePolicy.ChunkCellWidth;
                            var y = ((i % (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellHeight)) - x) / this.m_ChunkSizePolicy.ChunkCellWidth; 
                            var z = i / (this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellHeight);

                            var render = reader.ReadBoolean();

                            if (!render)
                            {
                                chunk.Cells[i].EdgePoint = 0;
                                chunk.Cells[i].BlockAssetName = null;
                                chunk.Cells[i].HeightMap = lookup[x, z];
                            }
                            else
                            {
                                var edgePoint = reader.ReadInt16();
                                var blockID = reader.ReadByte();

                                chunk.Cells[i].EdgePoint = edgePoint;
                                chunk.Cells[i].BlockAssetName = blockID == byte.MaxValue ? null : this.m_BlocksByID[blockID];
                                chunk.Cells[i].HeightMap = lookup[x, z];
                            }
                        }
                    }
                }
            }

            return chunk;
        }
    }
}