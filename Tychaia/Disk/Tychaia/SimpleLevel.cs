// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Tychaia.Data;

namespace Tychaia
{
    public class SimpleLevel : ILevel
    {
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly IAssetManager m_AssetManager;
        private readonly IFlowBundleSerializer m_FlowBundleSerializer;
        private string m_Path;

        public SimpleLevel(
            IChunkSizePolicy chunkSizePolicy,
            IAssetManagerProvider assetManagerProvider,
            IFlowBundleSerializer flowBundleSerializer,
            string name,
            string path)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_FlowBundleSerializer = flowBundleSerializer;
            this.m_Path = path;
        }

        private string GetName(RuntimeChunk chunk)
        {
            return chunk.X + "." + chunk.Y + "." + chunk.Z;
        }

        private string GetName(long x, long y, long z)
        {
            return x + "." + y + "." + z;
        }

        public void ScanChunks()
        {
        }

        public bool HasChunk(RuntimeChunk chunk)
        {
            return File.Exists(Path.Combine(this.m_Path, this.GetName(chunk)));
        }

        public bool HasChunk(long x, long y, long z)
        {
            return File.Exists(Path.Combine(this.m_Path, this.GetName(x, y, z)));
        }

        public void LoadChunk(RuntimeChunk runtimeChunk)
        {
            using (var file = new FileStream(
                Path.Combine(this.m_Path, this.GetName(runtimeChunk)), FileMode.Open))
            {
                var serializer = new TychaiaDataSerializer();
                var chunk = new Chunk();
                serializer.Deserialize(file, chunk, typeof(Chunk));
                for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                for (var y = 0; y < this.m_ChunkSizePolicy.ChunkCellHeight; y++)
                for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                    runtimeChunk.Cells[x, y, z] = chunk.Cells[
                        x +
                        y * this.m_ChunkSizePolicy.ChunkCellWidth +
                        z * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellHeight];
                runtimeChunk.GeneratedIndices = chunk.Indexes;
                if (chunk.Vertexes == null)
                {
                    runtimeChunk.GeneratedVertexes = new VertexPositionTexture[0];
                }
                else
                {
                    runtimeChunk.GeneratedVertexes = new VertexPositionTexture[chunk.Vertexes.Length];
                    for (var i = 0; i < chunk.Vertexes.Length; i++)
                        runtimeChunk.GeneratedVertexes[i] = new VertexPositionTexture(
                            new Vector3(chunk.Vertexes[i].X, chunk.Vertexes[i].Y, chunk.Vertexes[i].Z),
                            new Vector2(chunk.Vertexes[i].U, chunk.Vertexes[i].V));
                }
                runtimeChunk.Generated = true;
            }
        }

        public void SaveChunk(RuntimeChunk runtimeChunk)
        {
            using (var file = new FileStream(
                Path.Combine(this.m_Path, this.GetName(runtimeChunk)), FileMode.Create))
            {
                var chunk = new Chunk
                {
                    X = runtimeChunk.X,
                    Y = runtimeChunk.Y,
                    Z = runtimeChunk.Z,
                    Cells = new Cell[
                        this.m_ChunkSizePolicy.ChunkCellWidth *
                        this.m_ChunkSizePolicy.ChunkCellHeight *
                        this.m_ChunkSizePolicy.ChunkCellDepth],
                    Indexes = runtimeChunk.GeneratedIndices,
                    Vertexes = new Vertex[runtimeChunk.GeneratedVertexes.Length]
                };
                for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                for (var y = 0; y < this.m_ChunkSizePolicy.ChunkCellHeight; y++)
                for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                    chunk.Cells[
                        x +
                        y * this.m_ChunkSizePolicy.ChunkCellWidth +
                        z * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellHeight] = runtimeChunk.Cells[x, y, z];
                for (var i = 0; i < runtimeChunk.GeneratedVertexes.Length; i++)
                    chunk.Vertexes[i] = new Vertex
                    {
                        X = runtimeChunk.GeneratedVertexes[i].Position.X,
                        Y = runtimeChunk.GeneratedVertexes[i].Position.Y,
                        Z = runtimeChunk.GeneratedVertexes[i].Position.Z,
                        U = runtimeChunk.GeneratedVertexes[i].TextureCoordinate.X,
                        V = runtimeChunk.GeneratedVertexes[i].TextureCoordinate.Y,
                    };
                var serializer = new TychaiaDataSerializer();
                serializer.Serialize(file, chunk);
            }
        }

        public void SaveChunk(long _x, long _y, long _z, Cell[,,] data)
        {
            using (var file = new FileStream(
                Path.Combine(this.m_Path, this.GetName(_x, _y, _z)), FileMode.Create))
            {
                var chunk = new Chunk
                {
                    X = _x,
                    Y = _y,
                    Z = _z,
                    Cells = new Cell[
                        this.m_ChunkSizePolicy.ChunkCellWidth *
                        this.m_ChunkSizePolicy.ChunkCellHeight *
                        this.m_ChunkSizePolicy.ChunkCellDepth],
                    Indexes = new int[0],
                    Vertexes = new Vertex[0]
                };
                for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                for (var y = 0; y < this.m_ChunkSizePolicy.ChunkCellHeight; y++)
                for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                    chunk.Cells[
                        x +
                        y * this.m_ChunkSizePolicy.ChunkCellWidth +
                        z * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellHeight] = data[x, y, z];
                var serializer = new TychaiaDataSerializer();
                serializer.Serialize(file, chunk);
            }
        }
    }
}
