// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.Linq;
using Protogame;

namespace Tychaia.Network
{
    public class TychaiaServerWorld
    {
        private readonly Dictionary<MxClient, List<IServerEntity>> m_Entities;

        private readonly TychaiaServer m_Server;

        private readonly ServerChunkManager m_ServerChunkManager;

        public TychaiaServerWorld(TychaiaServer server, ServerChunkManager serverChunkManager)
        {
            this.m_Server = server;
            this.m_Entities = new Dictionary<MxClient, List<IServerEntity>>();
            
            // TODO: Move server chunk manager into entities list.
            this.m_ServerChunkManager = serverChunkManager;

            server.ListenForMessage("user input", this.OnUserInput);
        }

        public void AddPlayer(MxClient client, string playerName)
        {
            var entities = this.GetListForClient(client);

            entities.Add(
                new PlayerServerEntity(this.m_Server, client, this.m_Server.GetUniqueIDForClient(client))
                {
                    Name = playerName
                });
        }

        public void ChangePlayerName(MxClient client, string newName)
        {
            var entities = this.GetListForClient(client);

            var player = entities.OfType<PlayerServerEntity>().First();

            player.Name = newName;
        }

        public void Update()
        {
            foreach (var entity in this.m_Entities.SelectMany(kv => kv.Value))
            {
                entity.Update();
            }

            this.m_ServerChunkManager.Update();
        }

        private List<IServerEntity> GetListForClient(MxClient client)
        {
            if (!this.m_Entities.ContainsKey(client))
            {
                this.m_Entities[client] = new List<IServerEntity>();
            }

            return this.m_Entities[client];
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
    }
}