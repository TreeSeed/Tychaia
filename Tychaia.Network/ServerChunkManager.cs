// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Protogame;
using Tychaia.Globals;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class ServerChunkManager : IServerEntity
    {
        private readonly IChunkGenerator m_ChunkGenerator;

        private readonly IChunkOctreeFactory m_ChunkOctreeFactory;

        private readonly ConcurrentQueue<ChunkRequest> m_RequestedChunks;

        private readonly TychaiaServer m_Server;
        
        private readonly IChunkSizePolicy m_ChunkSizePolicy;

        private Thread m_GenerationThread;

        public ServerChunkManager(
            TychaiaServer server, 
            IChunkOctreeFactory chunkOctreeFactory, 
            IChunkGenerator chunkGenerator,
            IChunkSizePolicy chunkSizePolicy)
        {
            this.m_Server = server;
            this.m_ChunkOctreeFactory = chunkOctreeFactory;
            this.m_ChunkGenerator = chunkGenerator;
            this.m_ChunkSizePolicy = chunkSizePolicy;
            this.m_RequestedChunks = new ConcurrentQueue<ChunkRequest>();

            this.m_ChunkGenerator.InputDisconnect();
        }

        public ChunkOctree<ServerChunk> Octree { get; private set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public void ApplyDelta(BinaryReader reader)
        {
        }

        public void CalculateDelta(BinaryWriter writer, IServerEntity fromOther)
        {
        }

        public IServerEntity Snapshot()
        {
            return null;
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
        
        public void RequireChunk(long x, long y, long z)
        {
            if (this.Octree != null)
            {
                var serverChunk = this.Octree.Get(x, y, z);
                if (serverChunk != null && serverChunk.Cells != null)
                {
                    return;
                }
            }

            this.m_RequestedChunks.Enqueue(new ChunkRequest
            {
                X = x,
                Y = y,
                Z = z
            });
        }

        private void ThreadedUpdate()
        {
            this.m_ChunkGenerator.InputConnect();

            this.Octree = this.m_ChunkOctreeFactory.CreateChunkOctree<ServerChunk>();

            while (true)
            {
                ChunkRequest request;
                if (!this.m_RequestedChunks.TryDequeue(out request))
                {
                    Thread.Sleep(10);
                    continue;
                }

                var existing = this.Octree.Get(request.X, request.Y, request.Z);
                if (existing == null)
                {
                    var chunk = new ServerChunk(request.X, request.Y, request.Z);
                    this.Octree.Set(chunk);
                    this.m_ChunkGenerator.Generate(
                        chunk,
                        () => 
                        {
                        });
                }
            }
        }
    }
}