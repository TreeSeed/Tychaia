// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ProtoBuf;
using Protogame;

namespace Tychaia.Network
{
    public class TychaiaClient : INetworkAPI
    {
        private readonly Dictionary<string, Action<string>> m_MessageEvents;

        private readonly MxDispatcher m_MxDispatcher;

        public TychaiaClient(int port)
        {
            this.m_MxDispatcher = new MxDispatcher(port);
            this.m_MxDispatcher.MessageReceived += this.OnMessageReceived;
            this.m_MessageEvents = new Dictionary<string, Action<string>>();
        }

        public void Connect(IPEndPoint endpoint)
        {
            this.m_MxDispatcher.Connect(endpoint);
        }

        public void ListenForMessage(string type, Action<string> callback)
        {
            if (this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback already registered");
            }

            this.m_MessageEvents[type] = callback;
        }

        public byte[] LoadInitialState()
        {
            // TODO: Get the initial state.
            return null;
        }

        public void SendMessage(string type, string data)
        {
            using (var memory = new MemoryStream())
            {
                Serializer.Serialize(memory, new TychaiaInternalMessage { Type = type, Data = data });
                var length = (int)memory.Position;
                memory.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[length];
                memory.Read(bytes, 0, length);

                foreach (var endpoint in this.m_MxDispatcher.Endpoints)
                {
                    this.m_MxDispatcher.Send(endpoint, bytes);
                }
            }
        }

        public void Update()
        {
            this.m_MxDispatcher.Update();
        }

        private void OnMessageReceived(object sender, MxMessageEventArgs e)
        {
            using (var memory = new MemoryStream(e.Payload))
            {
                var message = Serializer.Deserialize<TychaiaInternalMessage>(memory);

                if (this.m_MessageEvents.ContainsKey(message.Type))
                {
                    this.m_MessageEvents[message.Type](message.Data);
                }
            }
        }
    }
}