// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Net;
using Protogame;

namespace Tychaia.Network
{
    public class TychaiaClient : IClientNetworkAPI
    {
        private readonly Dictionary<string, Action<MxClient, byte[]>> m_MessageEvents;

        private readonly MxDispatcher m_MxDispatcher;

        private DateTime m_LastUpdateCall;

        private DateTime m_LastDisconnectionWarningTime;

        public TychaiaClient(int port)
        {
            this.m_MxDispatcher = new MxDispatcher(port);
            this.m_MxDispatcher.MessageReceived += this.OnMessageReceived;
            this.m_MxDispatcher.ClientDisconnectWarning += this.OnClientDisconnectWarning;
            this.m_MxDispatcher.ClientDisconnected += this.OnClientDisconnected;
            this.m_MessageEvents = new Dictionary<string, Action<MxClient, byte[]>>();

            this.PlayersInGame = new string[0];
            this.m_LastUpdateCall = new DateTime(1970, 1, 1, 0, 0, 0);
            this.m_LastDisconnectionWarningTime = new DateTime(1970, 1, 1, 0, 0, 0);

            this.ListenForMessage(
                "player list", 
                (mxc, data) =>
                {
                    var list = InMemorySerializer.Deserialize<PlayerList>(data);
                    this.PlayersInGame = list.Players ?? new string[0];
                });
        }

        public double DisconnectingForSeconds { get; private set; }

        public bool IsPotentiallyDisconnecting { get; private set; }

        public bool IsDisconnected { get; private set; }

        public string[] PlayersInGame { get; private set; }

        public void Connect(IPEndPoint endpoint)
        {
            this.m_MxDispatcher.Connect(endpoint);
        }

        public void Close()
        {
            this.m_MxDispatcher.Close();
        }

        public void ListenForMessage(string type, Action<MxClient, byte[]> callback)
        {
            if (this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback already registered");
            }

            this.m_MessageEvents[type] = callback;
        }

        public void StopListeningForMessage(string type)
        {
            if (!this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback not registered");
            }

            this.m_MessageEvents.Remove(type);
        }

        public byte[] LoadInitialState()
        {
            // TODO: Get the initial state.
            return null;
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
            if ((DateTime.Now - this.m_LastDisconnectionWarningTime).TotalSeconds > 1)
            {
                this.IsPotentiallyDisconnecting = false;
            }

            if ((DateTime.Now - this.m_LastUpdateCall).TotalMilliseconds > 1000 / 30)
            {
                this.m_MxDispatcher.Update();
                this.m_LastUpdateCall = DateTime.Now;
            }
        }

        private void OnClientDisconnectWarning(object sender, MxDisconnectEventArgs e)
        {
            this.IsPotentiallyDisconnecting = true;
            this.m_LastDisconnectionWarningTime = DateTime.Now;
            this.DisconnectingForSeconds = e.DisconnectAccumulator / 30.0;
        }

        private void OnClientDisconnected(object sender, MxClientEventArgs e)
        {
            this.IsDisconnected = true;
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