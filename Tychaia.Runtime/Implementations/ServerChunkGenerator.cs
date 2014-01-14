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

        private readonly ThreadedTaskPipeline<ChunkGenerationRequest> m_GeneratorPipeline;

        private readonly ThreadedTaskPipeline<ChunkConversionRequest> m_ConverterPipeline;

        private readonly ThreadedTaskPipeline<ChunkCompressionRequest> m_CompressorPipeline;

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

            this.m_GeneratorPipeline = new ThreadedTaskPipeline<ChunkGenerationRequest>();
            this.m_ConverterPipeline = new ThreadedTaskPipeline<ChunkConversionRequest>(false);
            this.m_CompressorPipeline = new ThreadedTaskPipeline<ChunkCompressionRequest>(false);

            this.m_Generator = generatorResolver.GetGeneratorForGame();
            this.m_Generator.SetSeed(10000);

            new Thread(this.GeneratorRun)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            }.Start();

            new Thread(this.ConverterRun)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            }.Start();

            new Thread(this.CompressorRun)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            }.Start();
        }

        private void CompressorRun()
        {
            this.m_CompressorPipeline.OutputConnect();

            while (true)
            {
                var request = this.m_CompressorPipeline.Take();
                var chunk = request.Chunk;

                var start = DateTime.Now;

                chunk.CompressedData = this.m_ChunkCompressor.Compress(request.Converted);

                Console.WriteLine(
                    "Compressed chunk {0}, {1}, {2} in {3}",
                    chunk.X,
                    chunk.Y,
                    chunk.Z,
                    DateTime.Now - start);

                if (request.Callback != null)
                {
                    request.Callback();
                }
            }
        }

        private void ConverterRun()
        {
            this.m_ConverterPipeline.OutputConnect();
            this.m_CompressorPipeline.InputConnect();

            while (true)
            {
                var request = this.m_ConverterPipeline.Take();

                var start = DateTime.Now;
                
                var converted = this.m_ChunkConverter.ToChunk(request.Chunk);
                
                Console.WriteLine(
                    "Converted chunk {0}, {1}, {2} in {3}",
                    request.Chunk.X,
                    request.Chunk.Y,
                    request.Chunk.Z,
                    DateTime.Now - start);

                this.m_CompressorPipeline.Put(
                    new ChunkCompressionRequest
                    {
                        Callback = request.Callback,
                        Chunk = request.Chunk,
                        Converted = converted
                    });
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public void Generate(IChunk chunk, Action callback)
        {
            this.m_GeneratorPipeline.Put(new ChunkGenerationRequest { Callback = callback, Chunk = chunk });
        }

        public void InputConnect()
        {
            this.m_GeneratorPipeline.InputConnect();
        }

        public void InputDisconnect()
        {
            this.m_GeneratorPipeline.InputDisconnect();
        }

        private void GeneratorRun()
        {
            this.m_GeneratorPipeline.OutputConnect();
            this.m_ConverterPipeline.InputConnect();

            while (true)
            {
                var request = this.m_GeneratorPipeline.Take();
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

                this.m_ConverterPipeline.Put(
                    new ChunkConversionRequest
                    {
                        Callback = request.Callback,
                        Chunk = request.Chunk
                    });
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }

    internal class ChunkCompressionRequest
    {
        public Action Callback { get; set; }

        public IChunk Chunk { get; set; }

        public Chunk Converted { get; set; }
    }

    internal class ChunkConversionRequest
    {
        public Action Callback { get; set; }

        public IChunk Chunk { get; set; }
    }
}