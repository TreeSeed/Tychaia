// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Protogame;

namespace Tychaia.Network
{
    public class ServerClientManager
    {
        private readonly TychaiaServerWorld m_World;
    
        private readonly TychaiaServer m_Server;
        
        public ServerClientManager(
            IServerFactory serverFactory,
            TychaiaServer server,
            TychaiaServerWorld world,
            int uniqueID,
            string initialPlayerName,
            MxClient client)
        {
            this.m_Server = server;
            this.m_World = world;
        
            this.UniqueID = uniqueID;
            this.PlayerName = initialPlayerName;
            this.MxClient = client;
            this.Entities = new List<IServerEntity>();
            this.ClientChunkStateManager = serverFactory.CreateClientChunkStateManager(client);
        }

        public int UniqueID { get; set; }
        
        public string PlayerName { get; set; }
        
        public MxClient MxClient { get; set; }
        
        public ClientChunkStateManager ClientChunkStateManager { get; set; }
        
        public List<IServerEntity> Entities { get; set; }
        
        public PlayerServerEntity Player { get; set; }
        
        public void Update()
        {
            // Octree is created by server manager thread, we can't do anything until it's set.
            if (this.m_World.Octree == null || this.Player == null)
            {
                return;
            }
        
            this.ClientChunkStateManager.RecalculateDesiredChunks(
                this.Player,
                this.m_World.Octree,
                (x, y, z) =>
                {
                    var chunk = this.m_World.Octree.Get(x, y, z);
                    
                    if (chunk == null || chunk.CompressedData == null)
                    {
                        // We can't send anything yet, but we know the server chunk manager
                        // will already have this chunk queued, so we don't need to do anything
                        // here.
                        return;
                    }
                    
                    this.ClientChunkStateManager.Octree.Set(chunk);
                    
                    this.m_Server.SendMessage(
                        "chunk available", 
                        chunk.CompressedData,
                        this.MxClient,
                        true);
                });
        }
    }
}
