// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Parameters;
using Protogame;

namespace Tychaia.Network
{
    public class TychaiaServer : INetworkAPI
    {
        private readonly Dictionary<string, Action<MxClient, byte[]>> m_MessageEvents;

        private readonly MxDispatcher m_MxDispatcher;

        private readonly Dictionary<MxClient, string> m_PlayerLookup;

        private readonly Dictionary<MxClient, int> m_UniqueIDLookup;

        private int m_UniqueIDIncrementer;

        private TychaiaServerWorld m_World;

        public TychaiaServer(int realtimePort, int reliablePort)
        {
            this.m_MxDispatcher = new MxDispatcher(realtimePort, reliablePort);
            this.m_MxDispatcher.MessageReceived += this.OnMessageReceived;
            this.m_MessageEvents = new Dictionary<string, Action<MxClient, byte[]>>();
            this.m_PlayerLookup = new Dictionary<MxClient, string>();
            this.m_UniqueIDLookup = new Dictionary<MxClient, int>();
            this.m_UniqueIDIncrementer = 1;

            this.ListenForMessage(
                "join", 
                (client, playerName) =>
                {
                    // The client will repeatedly send join messages until we confirm.
                    if (this.m_PlayerLookup.ContainsKey(client))
                    {
                        return;
                    }

                    var uniqueID = this.m_UniqueIDIncrementer++;

                    Console.WriteLine("Detected \"" + Encoding.ASCII.GetString(playerName) + "\" has joined");
                    this.SendMessage("join confirm", BitConverter.GetBytes(uniqueID));
                    this.m_PlayerLookup.Add(client, Encoding.ASCII.GetString(playerName));
                    this.m_UniqueIDLookup.Add(client, uniqueID);
                    this.m_World.AddPlayer(client, Encoding.ASCII.GetString(playerName));
                });

            this.ListenForMessage(
                "change name", 
                (client, newPlayerName) =>
                {
                    // Check to make sure this client is joined.
                    if (!this.m_PlayerLookup.ContainsKey(client))
                    {
                        return;
                    }

                    var existingName = this.m_PlayerLookup[client];
                    var newName = Encoding.ASCII.GetString(newPlayerName);

                    this.m_PlayerLookup[client] = newName;
                    Console.WriteLine("\"" + existingName + "\" has changed their name to \"" + newName + "\"");
                    this.m_World.ChangePlayerName(client, newName);
                });
        }

        public string[] PlayersInGame
        {
            get
            {
                return this.m_PlayerLookup.Values.ToArray();
            }
        }

        public int GetUniqueIDForClient(MxClient client)
        {
            return this.m_UniqueIDLookup[client];
        }

        public void ListenForMessage(string type, Action<MxClient, byte[]> callback)
        {
            if (this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback already registered");
            }

            this.m_MessageEvents[type] = callback;
        }

        public void SendMessage(string type, byte[] data, bool reliable = false)
        {
            var bytes = InMemorySerializer.Serialize(new TychaiaInternalMessage { Type = type, Data = data });

            foreach (var endpoint in this.m_MxDispatcher.Endpoints)
            {
                this.m_MxDispatcher.Send(endpoint, bytes, reliable);
            }
        }

        public void StartWorld(IKernel kernel)
        {
            this.m_World = kernel.Get<TychaiaServerWorld>(new ConstructorArgument("server", this, true));
        }

        public void StopListeningForMessage(string type)
        {
            if (!this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback not registered");
            }

            this.m_MessageEvents.Remove(type);
        }

        public void Update()
        {
            // TODO: Send a real world state.
            this.SendMessage(
                "player list", 
                InMemorySerializer.Serialize(
                    new PlayerList { Players = this.m_PlayerLookup.Select(x => x.Value).ToArray() }));

            if (this.m_World != null)
            {
                this.m_World.Update();
            }

            this.m_MxDispatcher.Update();
        }

        private void OnMessageReceived(object sender, MxMessageEventArgs e)
        {
            var message = InMemorySerializer.Deserialize<TychaiaInternalMessage>(e.Payload);

            if (this.m_MessageEvents.ContainsKey(message.Type))
            {
                this.m_MessageEvents[message.Type](e.Client, message.Data);
            }
        }
    }
}