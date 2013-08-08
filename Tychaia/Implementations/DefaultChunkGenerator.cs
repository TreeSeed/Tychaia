using System;
using Tychaia.Threading;
using System.Threading;
using Tychaia.ProceduralGeneration;
using System.IO;
using Protogame;
using Tychaia.ProceduralGeneration.Blocks;

namespace Tychaia
{
    public class DefaultChunkGenerator : IChunkGenerator
    {
        private Thread m_Thread;
        private ThreadedTaskPipeline<Chunk> m_Pipeline;
        private RuntimeLayer m_ResultLayer;
        private IChunkSizePolicy m_ChunkSizePolicy;
        private IAssetManager m_AssetManager;
        
        public DefaultChunkGenerator(
            IChunkSizePolicy chunkSizePolicy,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
            this.m_Pipeline = new ThreadedTaskPipeline<Chunk>();
            this.m_Thread = new Thread(this.Run);
            this.m_Thread.IsBackground = true;
            this.m_Thread.Start();
            
            StorageLayer[] layers;
            using (var reader = new StreamReader("WorldConfig.xml"))
                layers = StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResultBlocks)
                {
                    m_ResultLayer = StorageAccess.ToRuntime(layer);
                    break;
                }
                
            m_ResultLayer.SetSeed(10000);
        }
        
        private void Run()
        {
            this.m_Pipeline.OutputConnect();
            
            while (true)
            {
                var chunk = this.m_Pipeline.Take();
                int computations;
                
                var blocks = (BlockInfo[])m_ResultLayer.GenerateData(
                    chunk.X / this.m_ChunkSizePolicy.CellVoxelWidth,
                    chunk.Z / this.m_ChunkSizePolicy.CellVoxelDepth,
                    chunk.Y / this.m_ChunkSizePolicy.CellVoxelHeight,
                    this.m_ChunkSizePolicy.ChunkCellWidth,
                    this.m_ChunkSizePolicy.ChunkCellHeight,
                    this.m_ChunkSizePolicy.ChunkCellDepth,
                    out computations);
                for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                for (var y = 0; y < this.m_ChunkSizePolicy.ChunkCellHeight; y++)
                for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                {
                    var info = blocks[
                        x +
                        z * this.m_ChunkSizePolicy.ChunkCellWidth +
                        y * this.m_ChunkSizePolicy.ChunkCellWidth * this.m_ChunkSizePolicy.ChunkCellHeight];
                    if (info.BlockAssetName == null)
                        continue;
                    chunk.Blocks[x, y, z] = this.m_AssetManager.Get<BlockAsset>(info.BlockAssetName);
                }
            }
        }
        
        public void Generate(Chunk chunk)
        {
            this.m_Pipeline.Put(chunk);
        }
    }
}

