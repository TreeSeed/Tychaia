// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;
using Tychaia.Asset;
using Tychaia.Data;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;
using Tychaia.Threading;

namespace Tychaia.Runtime
{
    public class DefaultChunkGenerator : IChunkGenerator
    {
        private readonly IAssetManager m_AssetManager;

        private readonly IChunkSizePolicy m_ChunkSizePolicy;

        private readonly IGenerator m_Generator;

        private readonly ThreadedTaskPipeline<ChunkGenerationRequest> m_Pipeline;

        private readonly TextureAtlasAsset m_TextureAtlasAsset;

        public DefaultChunkGenerator(
            IChunkSizePolicy chunkSizePolicy, 
            IAssetManagerProvider assetManagerProvider, 
            IGeneratorResolver generatorResolver)
        {
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_TextureAtlasAsset = this.m_AssetManager.Get<TextureAtlasAsset>("atlas");
            this.m_Pipeline = new ThreadedTaskPipeline<ChunkGenerationRequest>();
            this.m_Generator = generatorResolver.GetGeneratorForGame();
            this.m_Generator.SetSeed(10000);

            var thread = new Thread(this.Run) { IsBackground = true, Priority = ThreadPriority.Highest };
            thread.Start();
        }

        public void Generate(IChunk chunk, Action callback)
        {
            this.m_Pipeline.Put(new ChunkGenerationRequest()
            {
                Callback = callback,
                Chunk = chunk
            });
        }

        private void Run()
        {
            this.m_Pipeline.OutputConnect();

            while (true)
            {
                var request = this.m_Pipeline.Take();
                var chunk = request.Chunk;
                int computations;

                // Generate the actual data using the procedural generation library.
                var blocks =
                    new BlockAsset[this.m_ChunkSizePolicy.ChunkCellWidth, this.m_ChunkSizePolicy.ChunkCellHeight, 
                        this.m_ChunkSizePolicy.ChunkCellDepth];
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
                for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                {
                    for (var y = 0; y < this.m_ChunkSizePolicy.ChunkCellHeight; y++)
                    {
                        for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                        {
                            var idx = x + (z * this.m_ChunkSizePolicy.ChunkCellWidth)
                                      + (y * this.m_ChunkSizePolicy.ChunkCellWidth
                                         * this.m_ChunkSizePolicy.ChunkCellHeight);
                            var info = chunk.Cells[idx];
                            if (info.BlockAssetName == null)
                            {
                                continue;
                            }

                            blocks[x, y, z] = this.m_AssetManager.Get<BlockAsset>(info.BlockAssetName);
                        }
                    }
                }

                // Now also generate the vertexes / indices in this thread.
                var vertexes = new List<VertexPositionTexture>();
                var indices = new List<int>();
                for (var x = 0; x < this.m_ChunkSizePolicy.ChunkCellWidth; x++)
                {
                    for (var y = 0; y < this.m_ChunkSizePolicy.ChunkCellHeight; y++)
                    {
                        for (var z = 0; z < this.m_ChunkSizePolicy.ChunkCellDepth; z++)
                        {
                            var block = blocks[x, y, z];
                            if (block == null)
                            {
                                continue;
                            }

                            var xi = x;
                            var yi = y;
                            var zi = z;
                            var idx = x + (z * this.m_ChunkSizePolicy.ChunkCellWidth)
                                      + (y * this.m_ChunkSizePolicy.ChunkCellWidth
                                         * this.m_ChunkSizePolicy.ChunkCellHeight);
                            block.BuildRenderList(
                                this.m_TextureAtlasAsset, 
                                x, 
                                y, 
                                z, 
                                chunk.Cells[idx].EdgeDetection, 
                                (xx, yy, zz) =>
                                {
                                    if (xi + xx < 0 || yi + yy < 0 || zi + zz < 0
                                        || xi + xx >= this.m_ChunkSizePolicy.ChunkCellWidth
                                        || yi + yy >= this.m_ChunkSizePolicy.ChunkCellHeight
                                        || zi + zz >= this.m_ChunkSizePolicy.ChunkCellDepth)
                                    {
                                        return null;
                                    }

                                    return blocks[xi + xx, yi + yy, zi + zz];
                                }, 
                                (xx, yy, zz, uvx, uvy) =>
                                {
                                    vertexes.Add(
                                        new VertexPositionTexture(
                                            new Vector3(
                                                (chunk.X / (float)this.m_ChunkSizePolicy.CellVoxelWidth) + xx, 
                                                (chunk.Y / (float)this.m_ChunkSizePolicy.CellVoxelHeight) + yy, 
                                                (chunk.Z / (float)this.m_ChunkSizePolicy.CellVoxelDepth) + zz), 
                                            new Vector2(uvx, uvy)));
                                    return vertexes.Count - 1;
                                }, 
                                indices.Add);
                        }
                    }
                }

                chunk.GeneratedVertexes = vertexes.ToArray();
                chunk.GeneratedIndices = indices.ToArray();
                chunk.Generated = true;

                if (request.Callback != null)
                {
                    request.Callback();
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}