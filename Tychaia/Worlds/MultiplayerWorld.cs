// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Xna.Framework;
using Protogame;
using Tychaia.Network;

namespace Tychaia
{
    public class MultiplayerWorld : IWorld
    {
        private readonly CanvasEntity m_CanvasEntity;

        private readonly Thread m_QueryServersThread;

        private readonly ListView m_ServersListView;

        private IGameContext m_GameContext;
        
        private volatile bool m_Terminating = false;

        public MultiplayerWorld(ISkin skin)
        {
            this.Entities = new List<IEntity>();

            this.m_ServersListView = new ListView();
            this.m_ServersListView.AddChild(new ServerListItem { Text = "Loading servers...", Valid = false });
            this.m_ServersListView.SelectedItemChanged += this.ServersListViewOnSelectedItemChanged;

            var backButton = new Button { Text = "Back" };
            backButton.Click += (sender, args) => this.m_GameContext.SwitchWorld<TitleWorld>();

            var buttonContainer = new HorizontalContainer();
            buttonContainer.AddChild(new EmptyContainer(), "*");
            buttonContainer.AddChild(backButton, "100");
            buttonContainer.AddChild(new EmptyContainer(), "*");

            var verticalContainer = new VerticalContainer();
            verticalContainer.AddChild(new EmptyContainer(), "*");
            verticalContainer.AddChild(this.m_ServersListView, "370");
            verticalContainer.AddChild(new EmptyContainer(), "10");
            verticalContainer.AddChild(buttonContainer, "24");
            verticalContainer.AddChild(new EmptyContainer(), "*");

            var horizontalContainer = new HorizontalContainer();
            horizontalContainer.AddChild(new EmptyContainer(), "*");
            horizontalContainer.AddChild(verticalContainer, "300");
            horizontalContainer.AddChild(new EmptyContainer(), "*");

            var canvas = new Canvas();
            canvas.SetChild(horizontalContainer);

            this.m_CanvasEntity = new CanvasEntity(skin, canvas);

            this.m_QueryServersThread = new Thread(this.QueryServers) { Name = "Query Servers", IsBackground = true };
            this.m_QueryServersThread.Start();
        }

        public List<IEntity> Entities { get; private set; }

        public void Dispose()
        {
            this.m_Terminating = true;
            this.m_QueryServersThread.Abort();
        }

        public void QueryServers()
        {
            while (!this.m_Terminating)
            {
                try
                {
                    var servers = TychaiaServerQuery.QueryServers();
    
                    lock (this.m_ServersListView)
                    {
                        this.m_ServersListView.RemoveAllChildren();
    
                        foreach (var server in servers)
                        {
                            this.m_ServersListView.AddChild(
                                new ServerListItem
                                {
                                    Text = server.name + "(" + server.host + ":" + server.port + ")", 
                                    Address = IPAddress.Parse(server.host.ToString()), // FIXME: Support non-IP addresses
                                    Port = int.Parse(server.port.ToString()), 
                                    Valid = true
                                });
                        }
                    }
    
                    Thread.Sleep(10 * 1000);
                }
                catch (WebException ex)
                {
                }
            }
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                return;
            }

            this.m_CanvasEntity.Render(gameContext, renderContext);
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            renderContext.GraphicsDevice.Clear(Color.Black);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_GameContext = gameContext;

            this.m_CanvasEntity.Update(gameContext, updateContext);
        }

        private void ServersListViewOnSelectedItemChanged(object sender, SelectedItemChangedEventArgs<ListItem> args)
        {
            var serverListItem = (ServerListItem)args.Item;

            if (!serverListItem.Valid)
            {
                return;
            }

            // TODO: Have a seperate button to connect.
            this.m_GameContext.SwitchWorld<IWorldFactory>(
                x => x.CreateConnectWorld(false, serverListItem.Address, serverListItem.Port));
        }

        private class ServerListItem : ListItem
        {
            public IPAddress Address { get; set; }

            public int Port { get; set; }

            public bool Valid { get; set; }
        }
    }
}