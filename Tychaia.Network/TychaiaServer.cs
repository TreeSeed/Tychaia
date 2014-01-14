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

        private TychaiaServerWorld m_World;

        public TychaiaServer(int realtimePort, int reliablePort)
        {
            this.m_MxDispatcher = new MxDispatcher(realtimePort, reliablePort);
            this.m_MxDispatcher.MessageReceived += this.OnMessageReceived;
            this.m_MxDispatcher.ClientDisconnected += this.OnClientDisconnected;
            this.m_MessageEvents = new Dictionary<string, Action<MxClient, byte[]>>();
        }

        public string[] PlayersInGame
        {
            get
            {
                return this.m_World.PlayersInGame;
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

        public void SendMessage(string type, byte[] data, MxClient client = null, bool reliable = false)
        {
            var bytes = InMemorySerializer.Serialize(new TychaiaInternalMessage { Type = type, Data = data });

            if (client == null)
            {
                foreach (var endpoint in this.m_MxDispatcher.Endpoints)
                {
                    this.m_MxDispatcher.Send(endpoint, bytes, reliable);
                }
            }
            else
            {
                this.m_MxDispatcher.Send(client.DualEndpoint, bytes, reliable);
            }
        }

        public void StartWorld(IKernel kernel)
        {
            kernel.Bind<TychaiaServer>().ToMethod(x => this);
            
            this.m_World = kernel.Get<TychaiaServerWorld>();
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
        
        private void OnClientDisconnected(object sender, MxClientEventArgs e)
        {
            if (this.m_World != null)
            {
                this.m_World.OnClientDisconnected(sender, e);
            }
        }
    }
}