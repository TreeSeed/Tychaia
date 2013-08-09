// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.IO;
using System.Threading;
using Protogame;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Blocks;
using Tychaia.Threading;

namespace Tychaia
{
    public class DefaultChunkGenerator : IChunkGenerator
    {
        private readonly IAssetManager m_AssetManager;
        private readonly IChunkSizePolicy m_ChunkSizePolicy;
        private readonly ThreadedTaskPipeline<Chunk> m_Pipeline;
        private readonly RuntimeLayer m_ResultLayer;

        public DefaultChunkGenerator(
            IChunkSizePolicy chunkSizePolicy,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_Pipeline = new ThreadedTaskPipeline<Chunk>();
            
            var thread = new Thread(this.Run) { IsBackground = true };
            thread.Start();

            StorageLayer[] layers;
            using (var reader = new StreamReader("WorldConfig.xml"))
                layers = StorageAccess.LoadStorage(reader);
            foreach (var layer in layers)
                if (layer.Algorithm is AlgorithmResultBlocks)
                {
                    this.m_ResultLayer = StorageAccess.ToRuntime(layer);
                    break;
                }

            this.m_ResultLayer.SetSeed(10000);
        }

        public void Generate(Chunk chunk)
        {
            this.m_Pipeline.Put(chunk);
        }

        private void Run()
        {
            this.m_Pipeline.OutputConnect();

            while (true)
            {
                var chunk = this.m_Pipeline.Take();
                int computations;

                var blocks = (BlockInfo[]) this.m_ResultLayer.GenerateData(
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

            // ReSharper disable once FunctionNeverReturns
        }
    }
}