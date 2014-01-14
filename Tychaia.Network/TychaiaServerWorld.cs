// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Globals;
using Tychaia.Runtime;

namespace Tychaia.Network
{
    public class TychaiaServerWorld
    {
        private readonly Dictionary<MxClient, ServerClientManager> m_ConnectedClients;

        private readonly IServerEntityFactory m_ServerEntityFactory;
        
        private readonly IServerFactory m_ServerFactory;

        private readonly TychaiaServer m_Server;

        private readonly ServerChunkManager m_ServerChunkManager;

        private readonly IPositionScaleTranslation m_PositionScaleTranslation;
        
        private readonly IPredeterminedChunkPositions m_PredeterminedChunkPositions;

        private int m_UniqueIDIncrementer;

        public TychaiaServerWorld(
            IServerEntityFactory serverEntityFactory,
            IServerFactory serverFactory,
            IPositionScaleTranslation positionScaleTranslation,
            IPredeterminedChunkPositions predeterminedChunkPositions,
            TychaiaServer server,
            ServerChunkManager serverChunkManager)
        {
            this.m_ServerEntityFactory = serverEntityFactory;
            this.m_ServerFactory = serverFactory;
            this.m_PositionScaleTranslation = positionScaleTranslation;
            this.m_PredeterminedChunkPositions = predeterminedChunkPositions;
            this.m_Server = server;
            this.m_ConnectedClients = new Dictionary<MxClient, ServerClientManager>();
            
            this.m_UniqueIDIncrementer = 1;

            // TODO: Move server chunk manager into entities list.
            this.m_ServerChunkManager = serverChunkManager;

            this.m_Server.ListenForMessage("user input", this.OnUserInput);

            this.m_Server.ListenForMessage(
                "join", 
                (client, playerName) =>
                {
                    // The client will repeatedly send join messages until we confirm.
                    if (this.m_ConnectedClients.ContainsKey(client))
                    {
                        return;
                    }

                    var uniqueID = this.m_UniqueIDIncrementer++;

                    Console.WriteLine("Detected \"" + Encoding.ASCII.GetString(playerName) + "\" has joined");
                    this.m_Server.SendMessage("join confirm", BitConverter.GetBytes(uniqueID));
                    var manager = this.m_ServerFactory.CreateServerClientManager(
                        this,
                        this.m_Server,
                        uniqueID,
                        Encoding.ASCII.GetString(playerName),
                        client);
                    this.m_ConnectedClients.Add(
                        client, 
                        manager);
                    this.AddPlayer(client, Encoding.ASCII.GetString(playerName));
                });

            server.ListenForMessage(
                "change name", 
                (client, newPlayerName) =>
                {
                    // Check to make sure this client is joined.
                    if (!this.m_ConnectedClients.ContainsKey(client))
                    {
                        return;
                    }

                    var existingName = this.m_ConnectedClients[client].PlayerName;
                    var newName = Encoding.ASCII.GetString(newPlayerName);

                    this.m_ConnectedClients[client].PlayerName = newName;
                    Console.WriteLine("\"" + existingName + "\" has changed their name to \"" + newName + "\"");
                    this.ChangePlayerName(client, newName);
                });
        }
        
        public string[] PlayersInGame
        {
            get
            {
                return this.m_ConnectedClients.Values.Select(x => x.PlayerName).ToArray();
            }
        }

        public ChunkOctree<ServerChunk> Octree
        {
            get
            {
                return this.m_ServerChunkManager.Octree;
            }
        }

        public void AddPlayer(MxClient client, string playerName)
        {
            var entities = this.GetListForClient(client);

            var playerEntity = this.m_ServerEntityFactory.CreatePlayerServerEntity(
                this.m_Server,
                this,
                client,
                this.m_ConnectedClients[client].UniqueID);
            playerEntity.Name = playerName;
            
            this.m_ConnectedClients[client].Player = playerEntity;

            entities.Add(playerEntity);
        }
        
        public void ChangePlayerName(MxClient client, string newName)
        {
            var entities = this.GetListForClient(client);

            var player = entities.OfType<PlayerServerEntity>().First();

            player.Name = newName;
        }

        public void Update()
        {
            // TODO: Send a real world state.
            this.m_Server.SendMessage(
                "player list", 
                InMemorySerializer.Serialize(
                    new PlayerList { Players = this.PlayersInGame }));
                    
            foreach (var client in this.m_ConnectedClients.Values)
            {
                client.Update();
            }

            foreach (var entity in this.m_ConnectedClients.Values.SelectMany(x => x.Entities))
            {
                entity.Update();
            }

            this.RecalculateDesiredServerChunks();

            this.m_ServerChunkManager.Update();
        }
        
        public void OnClientDisconnected(object sender, MxClientEventArgs e)
        {
            // Check to make sure this client is joined.
            if (!this.m_ConnectedClients.ContainsKey(e.Client))
            {
                return;
            }
            
            // Remove the unique ID and player from the world.
            var entities = this.GetListForClient(e.Client);
            
            foreach (var player in entities.OfType<PlayerServerEntity>())
            {
                player.Leave();
            }
            
            entities.Clear();
            this.m_ConnectedClients.Remove(e.Client);
        }
        
        private void RecalculateDesiredServerChunks()
        {
            // Octree is created by server manager thread, we can't do anything until it's set.
            if (this.Octree == null)
            {
                return;
            }
            
            var chunks = new List<ChunkPos>();
        
            // For each player and the 0x0x0 position, calculate radius.
            foreach (var playerEntity in this.m_ConnectedClients.Values.Select(x => x.Player))
            {
                var current = this.Octree.Get((long)playerEntity.X, (long)playerEntity.Y, (long)playerEntity.Z);
            
                foreach (var l in this.m_PredeterminedChunkPositions.GetAbsolutePositions(new Vector3(
                    (float)current.X,
                    (float)current.Y,
                    (float)current.Z)))
                {
                    chunks.Add(
                        new ChunkPos
                        {
                            X = (long)l.X,
                            Y = (long)l.Y,
                            Z = (long)l.Z
                        });
                }
            }
            
            foreach (var l in this.m_PredeterminedChunkPositions.GetAbsolutePositions(new Vector3(0, 0, 0)))
            {
                chunks.Add(
                    new ChunkPos
                    {
                        X = (long)l.X,
                        Y = (long)l.Y,
                        Z = (long)l.Z
                    });
            }
            
            // Check if each of the chunks is already in the octree.
            foreach (var pos in chunks.ToArray())
            {
                if (this.Octree.Get(pos.X, pos.Y, pos.Z) != null)
                {
                    chunks.Remove(pos);
                }
            }
            
            // Callback for required chunks.
            foreach (var chunk in chunks)
            {
                this.m_ServerChunkManager.RequireChunk(chunk.X, chunk.Y, chunk.Z);
            }
        }

        private List<IServerEntity> GetListForClient(MxClient client)
        {
            return this.m_ConnectedClients[client].Entities;
        }

        private void OnUserInput(MxClient client, byte[] data)
        {
            var userInput = InMemorySerializer.Deserialize<UserInput>(data);

            // Find the player entity for this client.
            var entities = this.GetListForClient(client);
            var player = entities.OfType<PlayerServerEntity>().FirstOrDefault();
            if (player == null)
            {
                return;
            }

            switch (userInput.GetAction())
            {
                case UserInputAction.Move:
                    player.MoveInDirection(userInput.DirectionInDegrees);
                    break;
            }
        }
        
        private class ChunkPos
        {
            public long X { get; set; }
            
            public long Y { get; set; }
            
            public long Z { get; set; }
        }
    }
}