// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Concurrent;
using System.Threading;

namespace Tychaia.Runtime
{
    public class DefaultServerWorld : IServerWorld
    {
        private readonly IChunkOctreeFactory m_ChunkOctreeFactory;

        private readonly ILevelAPI m_LevelAPI;

        private readonly IChunkGenerator m_ChunkGenerator;

        private ChunkOctree m_ChunkOctree;

        private Thread m_GenerationThread;

        private ConcurrentQueue<PositionRequest> m_RequestedChunks;

        public DefaultServerWorld(
            IChunkOctreeFactory chunkOctreeFactory, 
            ILevelAPI levelAPI,
            IChunkGenerator chunkGenerator)
        {
            this.m_ChunkOctreeFactory = chunkOctreeFactory;
            this.m_LevelAPI = levelAPI;
            this.m_ChunkGenerator = chunkGenerator;
        }

        public RuntimeChunk GetChunk(long x, long y, long z)
        {
            return this.m_ChunkOctree.Get(x, y, z);
        }

        public void RequestChunk(long x, long y, long z)
        {
            this.m_RequestedChunks.Enqueue(new PositionRequest { X = x, Y = y, Z = z });
        }

        public void Update()
        {
            if (this.m_GenerationThread == null)
            {
                this.m_GenerationThread = new Thread(this.ThreadedUpdate);
                this.m_GenerationThread.IsBackground = true;
                this.m_GenerationThread.Name = "Server World Generation Thread";
                this.m_GenerationThread.Start();
            }
        }

        private void ThreadedUpdate()
        {
            this.m_ChunkOctree = this.m_ChunkOctreeFactory.CreateChunkOctree();

            var level = this.m_LevelAPI.NewLevel("test");

            while (true)
            {
                // TODO: Work out what chunks to generate ahead of time.
                PositionRequest request;
                if (!this.m_RequestedChunks.TryDequeue(out request))
                {
                    Thread.Sleep(10);
                }

                var existing = this.m_ChunkOctree.Get(request.X, request.Y, request.Z);
                if (existing != null)
                {
                    var chunk = new RuntimeChunk(request.X, request.Y, request.Z);
                    this.m_ChunkGenerator.Generate(chunk);
                }
            }
        }

        private class PositionRequest
        {
            public long X { get; set; }

            public long Y { get; set; }

            public long Z { get; set; }
        }
    }
}