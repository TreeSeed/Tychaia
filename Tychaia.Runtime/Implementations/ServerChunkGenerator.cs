// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Threading;
using Protogame;
using Tychaia.Asset;
using Tychaia.Data;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;
using Tychaia.Threading;

namespace Tychaia.Runtime
{
    public class ServerChunkGenerator : IChunkGenerator
    {
        private readonly IAssetManager m_AssetManager;

        private readonly IChunkSizePolicy m_ChunkSizePolicy;

        private readonly IEdgePointCalculator m_EdgePointCalculator;

        private readonly IGenerator m_Generator;

        private readonly ThreadedTaskPipeline<ChunkGenerationRequest> m_Pipeline;
        
        private readonly IChunkCompressor m_ChunkCompressor;
        
        private readonly IChunkConverter m_ChunkConverter;

        public ServerChunkGenerator(
            IChunkSizePolicy chunkSizePolicy, 
            IAssetManagerProvider assetManagerProvider, 
            IGeneratorResolver generatorResolver,
            IEdgePointCalculator edgePointCalculator,
            IChunkConverter chunkConverter,
            IChunkCompressor chunkCompressor)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_EdgePointCalculator = edgePointCalculator;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_ChunkCompressor = chunkCompressor;
            this.m_ChunkConverter = chunkConverter;

            this.m_Pipeline = new ThreadedTaskPipeline<ChunkGenerationRequest>();
            this.m_Generator = generatorResolver.GetGeneratorForGame();
            this.m_Generator.SetSeed(10000);

            var thread = new Thread(this.Run) { IsBackground = true, Priority = ThreadPriority.Highest };
            thread.Start();
        }

        public void Generate(IChunk chunk, Action callback)
        {
            this.m_Pipeline.Put(new ChunkGenerationRequest { Callback = callback, Chunk = chunk });
        }

        public void InputConnect()
        {
            this.m_Pipeline.InputConnect();
        }

        public void InputDisconnect()
        {
            this.m_Pipeline.InputDisconnect();
        }

        private void Run()
        {
            this.m_Pipeline.OutputConnect();

            while (true)
            {
                var request = this.m_Pipeline.Take();
                var chunk = request.Chunk;
                int computations;

                var start = DateTime.Now;

                // Generate the actual data using the procedural generation library.
                chunk.Cells =
                    (Cell[])
                    this.m_Generator.GenerateData(
                        chunk.X / this.m_ChunkSizePolicy.CellVoxelWidth, 
                        chunk.Z / this.m_ChunkSizePolicy.CellVoxelDepth, 
                        chunk.Y / this.m_ChunkSizePolicy.CellVoxelHeight, 
                        this.m_ChunkSizePolicy.ChunkCellWidth, 
                        this.m_ChunkSizePolicy.ChunkCellHeight, 
                        this.m_ChunkSizePolicy.ChunkCellDepth, 
                        out computations);

                // TODO: Move this into an algorithm in the generation
                for (var i = 0; i < chunk.Cells.Length; i++)
                {
                    chunk.Cells[i].EdgePoint =
                        this.m_EdgePointCalculator.CalculateEdgePoint(chunk.Cells[i].EdgeDetection).Compress();
                }

                Console.WriteLine(
                    "Generated chunk {0}, {1}, {2} in {3}",
                    chunk.X,
                    chunk.Y,
                    chunk.Z,
                    DateTime.Now - start);
                start = DateTime.Now;
                
                var converted = this.m_ChunkConverter.ToChunk(chunk);
                
                Console.WriteLine(
                    "Converted chunk {0}, {1}, {2} in {3}",
                    chunk.X,
                    chunk.Y,
                    chunk.Z,
                    DateTime.Now - start);
                start = DateTime.Now;

                chunk.CompressedData = this.m_ChunkCompressor.Compress(converted);

                Console.WriteLine(
                    "Compressed chunk {0}, {1}, {2} in {3}",
                    chunk.X,
                    chunk.Y,
                    chunk.Z,
                    DateTime.Now - start);
                start = DateTime.Now;
                
                if (request.Callback != null)
                {
                    request.Callback();
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}