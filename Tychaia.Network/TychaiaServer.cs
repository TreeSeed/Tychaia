// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;

namespace Tychaia.Network
{
    public class TychaiaServer : INetworkAPI
    {
        private readonly Dictionary<string, Action<MxClient, byte[]>> m_MessageEvents;

        private readonly MxDispatcher m_MxDispatcher;

        private readonly Dictionary<MxClient, string> m_PlayerLookup;

        public TychaiaServer(int port)
        {
            this.m_MxDispatcher = new MxDispatcher(port);
            this.m_MxDispatcher.MessageReceived += this.OnMessageReceived;
            this.m_MessageEvents = new Dictionary<string, Action<MxClient, byte[]>>();
            this.m_PlayerLookup = new Dictionary<MxClient, string>();

            this.ListenForMessage(
                "join", 
                (client, playerName) =>
                {
                    // The client will repeatedly send join messages until we confirm.
                    if (this.m_PlayerLookup.ContainsKey(client))
                    {
                        return;
                    }

                    Console.WriteLine("Detected \"" + Encoding.ASCII.GetString(playerName) + "\" has joined");
                    this.SendMessage("join confirm", playerName);
                    this.m_PlayerLookup.Add(client, Encoding.ASCII.GetString(playerName));
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
                });
        }

        public string[] PlayersInGame
        {
            get
            {
                return this.m_PlayerLookup.Values.ToArray();
            }
        }

        public void ListenForMessage(string type, Action<MxClient, byte[]> callback)
        {
            if (this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback already registered");
            }

            this.m_MessageEvents[type] = callback;
        }

        public void SendMessage(string type, byte[] data)
        {
            var bytes = InMemorySerializer.Serialize(new TychaiaInternalMessage { Type = type, Data = data });

            foreach (var endpoint in this.m_MxDispatcher.Endpoints)
            {
                this.m_MxDispatcher.Send(endpoint, bytes);
            }
        }

        public void Update()
        {
            // TODO: Send a real world state.
            this.SendMessage(
                "player list", 
                InMemorySerializer.Serialize(
                    new PlayerList { Players = this.m_PlayerLookup.Select(x => x.Value).ToArray() }));

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