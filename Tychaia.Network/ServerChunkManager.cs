// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Protogame;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class ServerChunkManager : IServerEntity
    {
        private readonly IChunkConverter m_ChunkConverter;

        private readonly IChunkCompressor m_ChunkCompressor;

        private readonly IChunkGenerator m_ChunkGenerator;

        private readonly IChunkOctreeFactory m_ChunkOctreeFactory;

        private readonly ConcurrentQueue<ChunkRequest> m_RequestedChunks;

        private readonly TychaiaServer m_Server;

        private Thread m_GenerationThread;

        public ServerChunkManager(
            TychaiaServer server, 
            IChunkOctreeFactory chunkOctreeFactory, 
            IChunkGenerator chunkGenerator, 
            IChunkConverter chunkConverter,
            IChunkCompressor chunkCompressor)
        {
            this.m_Server = server;
            this.m_ChunkOctreeFactory = chunkOctreeFactory;
            this.m_ChunkGenerator = chunkGenerator;
            this.m_ChunkConverter = chunkConverter;
            this.m_ChunkCompressor = chunkCompressor;
            this.m_RequestedChunks = new ConcurrentQueue<ChunkRequest>();

            this.m_ChunkGenerator.InputDisconnect();

            this.m_Server.ListenForMessage(
                "require chunk", 
                (client, data) =>
                {
                    var request = InMemorySerializer.Deserialize<ChunkRequest>(data);

                    if (this.Octree != null)
                    {
                        var serverChunk = this.Octree.Get(request.X, request.Y, request.Z);
                        if (serverChunk != null && serverChunk.Cells != null)
                        {
                            this.AnnounceChunk(client, serverChunk);
                            return;
                        }
                    }

                    this.m_RequestedChunks.Enqueue(request);
                });
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

        private void AnnounceChunk(MxClient client, ServerChunk serverChunk)
        {
            this.m_Server.SendMessage(
                "chunk available", 
                this.m_ChunkCompressor.Compress(this.m_ChunkConverter.ToChunk(serverChunk)),
                client,
                true);
        }

        private void AnnounceChunk(ServerChunk serverChunk)
        {
            this.m_Server.SendMessage(
                "chunk available",
                this.m_ChunkCompressor.Compress(this.m_ChunkConverter.ToChunk(serverChunk)),
                reliable: true);
        }

        private void ThreadedUpdate()
        {
            this.m_ChunkGenerator.InputConnect();

            this.Octree = this.m_ChunkOctreeFactory.CreateChunkOctree<ServerChunk>();

            while (true)
            {
                // TODO: Work out what chunks to generate ahead of time.
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
                    this.m_ChunkGenerator.Generate(chunk, () => this.AnnounceChunk(chunk));
                }
                else
                {
                    // This might have been double-queued depending on timing.
                    // We also need to check the Cells property because if it is null, then
                    // the node is set in the octree but hasn't been announced yet (it is
                    // still being generated).
                    if (existing.Cells != null)
                    {
                        this.AnnounceChunk(existing);
                    }
                }
            }
        }
    }
}