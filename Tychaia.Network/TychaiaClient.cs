﻿// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Protogame;

namespace Tychaia.Network
{
    public class TychaiaClient : IClientNetworkAPI
    {
        private readonly Dictionary<string, Action<MxClient, byte[]>> m_MessageEvents;

        private readonly MxDispatcher m_MxDispatcher;

        private DateTime m_LastDisconnectionWarningTime;

        private DateTime m_LastUpdateCall;

        private IProfiler m_Profiler;

        public TychaiaClient(int realtimePort, int reliablePort)
        {
            this.m_MxDispatcher = new MxDispatcher(realtimePort, reliablePort);
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

        ~TychaiaClient()
        {
            if (this.m_Profiler != null)
            {
                this.m_Profiler.DetachNetworkDispatcher(this.m_MxDispatcher);
            }
        }

        public double DisconnectingForSeconds { get; private set; }

        public bool IsDisconnected { get; private set; }

        public bool IsPotentiallyDisconnecting { get; private set; }

        public string[] PlayersInGame { get; private set; }

        public void AttachProfiler(IProfiler profiler)
        {
            if (profiler == null)
            {
                throw new ArgumentNullException("profiler");
            }

            this.m_Profiler = profiler;
            this.m_Profiler.AttachNetworkDispatcher(this.m_MxDispatcher);
        }

        public void Close()
        {
            this.m_MxDispatcher.Close();
        }

        public void Connect(DualIPEndPoint endpoint)
        {
            this.m_MxDispatcher.Connect(endpoint);
        }

        public void ListenForMessage(string type, Action<MxClient, byte[]> callback)
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

        public void StopListeningForMessage(string type)
        {
            if (!this.m_MessageEvents.ContainsKey(type))
            {
                return;
            }

            this.m_MessageEvents.Remove(type);
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