// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Ninject;
using Tychaia.Data;
using Tychaia.Globals;
using Tychaia.Network;
using Xunit;

namespace Tychaia.Runtime.Tests
{
    public class TerrainSurfaceCalculatorTests
    {
        [Fact]
        public void TestAccessCorrectCell()
        {
            var state = this.GetTestState();
            var calculator = state.Calculator;
            var octree = state.Octree;

            Func<long, long, int> heightMapFunc = (x, z) => (int)(x + (z * state.ChunkSizePolicy.ChunkCellWidth));

            var chunk = this.CreateServerChunk(state.ChunkSizePolicy, 0, 0, 0, heightMapFunc);

            octree.Set(chunk);

            for (var xx = 0; xx < state.ChunkSizePolicy.ChunkCellWidth; xx++)
            {
                for (var zz = 0; zz < state.ChunkSizePolicy.ChunkCellDepth; zz++)
                {
                    var x = xx * state.ChunkSizePolicy.CellVoxelWidth;
                    var z = zz * state.ChunkSizePolicy.CellVoxelDepth;

                    Assert.Equal(
                        heightMapFunc(xx, zz) * state.ChunkSizePolicy.CellVoxelDepth, 
                        calculator.GetSurfaceY(octree, x, z));
                }
            }
        }

        [Fact]
        public void TestAccessCorrectChunk()
        {
            var state = this.GetTestState();
            var calculator = state.Calculator;
            var octree = state.Octree;

            var chunk = this.CreateServerChunk(state.ChunkSizePolicy, 0, 0, 0, (x, z) => 0);

            var maxSize = state.ChunkSizePolicy.ChunkCellWidth * state.ChunkSizePolicy.CellVoxelWidth;

            octree.Set(chunk);

            Assert.Equal(0, calculator.GetSurfaceY(octree, 0, 0));

            Assert.Equal(0, calculator.GetSurfaceY(octree, maxSize - 1, 0));

            Assert.Equal(0, calculator.GetSurfaceY(octree, 0, maxSize - 1));

            Assert.Equal(0, calculator.GetSurfaceY(octree, maxSize - 1, maxSize - 1));

            Assert.Equal(null, calculator.GetSurfaceY(octree, -1, 0));

            Assert.Equal(null, calculator.GetSurfaceY(octree, -1, -1));

            Assert.Equal(null, calculator.GetSurfaceY(octree, 0, -1));

            Assert.Equal(null, calculator.GetSurfaceY(octree, maxSize, 0));

            Assert.Equal(null, calculator.GetSurfaceY(octree, 0, maxSize));

            Assert.Equal(null, calculator.GetSurfaceY(octree, maxSize, maxSize));
        }

        private ServerChunk CreateServerChunk(
            IChunkSizePolicy chunkSizePolicy, 
            long x, 
            long y, 
            long z, 
            Func<long, long, int> heightMapFunc)
        {
            var size = chunkSizePolicy.ChunkCellWidth * chunkSizePolicy.ChunkCellHeight * chunkSizePolicy.ChunkCellDepth;
            var chunk = new ServerChunk(x, y, z) { Cells = new Cell[size] };
            for (var xx = 0; xx < chunkSizePolicy.ChunkCellWidth; xx++)
            {
                for (var yy = 0; yy < chunkSizePolicy.ChunkCellHeight; yy++)
                {
                    for (var zz = 0; zz < chunkSizePolicy.ChunkCellDepth; zz++)
                    {
                        var i = xx + (yy * chunkSizePolicy.ChunkCellWidth)
                                + (zz * chunkSizePolicy.ChunkCellWidth * chunkSizePolicy.ChunkCellHeight);
                        chunk.Cells[i].HeightMap = heightMapFunc(xx, zz);
                    }
                }
            }

            return chunk;
        }

        private TestState GetTestState()
        {
            var state = new TestState();

            var kernel = new StandardKernel();
            kernel.Load<TychaiaGlobalIoCModule>();
            kernel.Load<TychaiaNetworkIoCModule>();
            kernel.Load<TychaiaRuntimeIoCModule>();

            state.ChunkSizePolicy = kernel.Get<IChunkSizePolicy>();
            state.Calculator = kernel.Get<ITerrainSurfaceCalculator>();
            state.Octree = kernel.Get<IChunkOctreeFactory>().CreateChunkOctree<ServerChunk>();

            return state;
        }

        private class TestState
        {
            public ITerrainSurfaceCalculator Calculator { get; set; }

            public IChunkSizePolicy ChunkSizePolicy { get; set; }

            public ChunkOctree<ServerChunk> Octree { get; set; }
        }
    }
}