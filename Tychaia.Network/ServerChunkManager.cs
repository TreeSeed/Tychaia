// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Protogame;
using Tychaia.Network.Entities;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class ServerChunkManager : IServerEntity
    {
        private readonly IChunkConverter m_ChunkConverter;

        private readonly IChunkGenerator m_ChunkGenerator;

        private readonly IChunkOctreeFactory m_ChunkOctreeFactory;

        private readonly ConcurrentQueue<ChunkRequest> m_RequestedChunks;

        private readonly TychaiaServer m_Server;

        private ChunkOctree<ServerChunk> m_ChunkOctree;

        private Thread m_GenerationThread;

        public ServerChunkManager(
            TychaiaServer server, 
            IChunkOctreeFactory chunkOctreeFactory, 
            IChunkGenerator chunkGenerator, 
            IChunkConverter chunkConverter)
        {
            this.m_Server = server;
            this.m_ChunkOctreeFactory = chunkOctreeFactory;
            this.m_ChunkGenerator = chunkGenerator;
            this.m_ChunkConverter = chunkConverter;
            this.m_RequestedChunks = new ConcurrentQueue<ChunkRequest>();

            this.m_ChunkGenerator.InputDisconnect();

            this.m_Server.ListenForMessage(
                "require chunk", 
                (client, data) =>
                {
                    var request = InMemorySerializer.Deserialize<ChunkRequest>(data);

                    if (this.m_ChunkOctree != null)
                    {
                        var serverChunk = this.m_ChunkOctree.Get(request.X, request.Y, request.Z);
                        if (serverChunk != null)
                        {
                            this.AnnounceChunk(serverChunk);
                            return;
                        }
                    }

                    this.m_RequestedChunks.Enqueue(request);
                });
        }

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

        private void AnnounceChunk(ServerChunk serverChunk)
        {
            this.m_Server.SendMessage(
                "chunk available", 
                InMemorySerializer.Serialize(this.m_ChunkConverter.ToChunk(serverChunk)));
        }

        private void ThreadedUpdate()
        {
            this.m_ChunkGenerator.InputConnect();

            this.m_ChunkOctree = this.m_ChunkOctreeFactory.CreateChunkOctree<ServerChunk>();

            while (true)
            {
                // TODO: Work out what chunks to generate ahead of time.
                ChunkRequest request;
                if (!this.m_RequestedChunks.TryDequeue(out request))
                {
                    Thread.Sleep(10);
                    continue;
                }

                var existing = this.m_ChunkOctree.Get(request.X, request.Y, request.Z);
                if (existing == null)
                {
                    var chunk = new ServerChunk(request.X, request.Y, request.Z);
                    this.m_ChunkOctree.Set(chunk);
                    this.m_ChunkGenerator.Generate(chunk, () => this.AnnounceChunk(chunk));
                }
                else
                {
                    // This might have been double-queued depending on timing.
                    this.AnnounceChunk(existing);
                }
            }
        }
    }
}