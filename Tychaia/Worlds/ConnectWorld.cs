// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Dx.Runtime;
using Microsoft.Xna.Framework;
using Ninject;
using Protogame;
using Tychaia.Network;

namespace Tychaia
{
    public class ConnectWorld : MenuWorld
    {
        private readonly IAssetManager m_AssetManager;
        private readonly I2DRenderUtilities m_2DRenderUtilities;
        
        private readonly FontAsset m_DefaultFont;
        private IPAddress m_Address;
        private int m_Port;
        private Action[] m_Actions;
        private int m_ActionStep;
        private string m_Message = string.Empty;
        private Process m_Process;
        private bool m_NormalShutdown = false;
    
        public ConnectWorld(
            IKernel kernel,
            IDxFactory dxFactory,
            ILevelAPI levelAPI,
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory,
            ISkin skin)
            : base(twodRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_2DRenderUtilities = twodRenderUtilities;
            
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_Address = IPAddress.Loopback;
            this.m_Port = 9091;
            ILocalNode node = null;
            GameState state = null;
            byte[] initial = null;
            var level = levelAPI.NewLevel((new Random()).Next().ToString());
            Action cleanup = () =>
            {
                this.m_NormalShutdown = true;
                kernel.Unbind<ILocalNode>();
                node.Leave();
                this.TerminateExistingProcess();
            };
            this.m_Actions = new Action[]
            {
                () => this.m_Message = "Closing old process...",
                () => this.TerminateExistingProcess(),
                () => this.m_Message = "Starting server...",
                () => this.StartServer(),
                () => this.m_Message = "Setting up kernel...",
                () => TychaiaTCPNetwork.SetupKernel(kernel, false, this.m_Address, this.m_Port),
                () => this.m_Message = "Creating distributed node...",
                () => node = dxFactory.CreateLocalNode(Caching.PushOnChange, Architecture.ServerClient),
                () => this.m_Message = "Binding node to kernel...",
                () => kernel.Bind<ILocalNode>().ToMethod(x => node),
                () => this.m_Message = "Joining network...",
                () => this.AttemptJoin(node),
                () => this.m_Message = "Retrieving reference to game state...",
                () => state = this.AttemptStateRetrieval(node),
                () => this.m_Message = "Joining game...",
                () => state.JoinGame(),
                () => this.m_Message = "Retrieving initial game state...",
                () => initial = state.LoadInitialState(),
                () => this.m_Message = "Starting client...",
                () => this.TargetWorld = this.GameContext.CreateWorld<IWorldFactory>(x => x.CreateTychaiaGameWorld(state, initial, cleanup))
            };
            
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => 
            {
                if (this.m_Process != null)
                {
                    try
                    {
                        this.m_Process.Kill();
                        this.m_Process = null;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (this.m_Process != null)
                {
                    try
                    {
                        this.m_Process.Kill();
                        this.m_Process = null;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            };
        }
        
        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);
            
            if (this.m_ActionStep < this.m_Actions.Length)
            {
                this.m_Actions[this.m_ActionStep++]();
            }
            
            if (this.m_ActionStep >= this.m_Actions.Length && this.TargetWorld == null)
            {
                throw new InvalidOperationException();
            }
        }
        
        public override void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            base.RenderBelow(gameContext, renderContext);

            if (renderContext.Is3DContext)
                return;

            this.m_2DRenderUtilities.RenderText(
                renderContext,
                new Vector2(400, 400),
                this.m_Message,
                this.m_DefaultFont);
        }
        
        private GameState AttemptStateRetrieval(ILocalNode node)
        {
            for (var i = 0; i < 5; i++)
            {
                var state = new Distributed<GameState>(node, GameState.NAME, true);
                if ((GameState)state != null)
                    return state;
                Thread.Sleep(1000);
            }
            
            throw new InvalidOperationException("Unable to retrieve game state.");
        }
        
        private void AttemptJoin(ILocalNode node)
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    // We can only call node.Join when we're sure we can make a connection.
                    // So we basically try to do a basic connection with TcpClient.
                    var client = new TcpClient();
                    client.Connect(new IPEndPoint(this.m_Address, this.m_Port));
                    client.Close();
                    
                    // If we got to here, then we can actually connect.
                    node.Join(null);
                    return;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(1000);
                }
            }
            
            throw new InvalidOperationException("Unable to join game.");
        }
        
        private void TerminateExistingProcess()
        {
            if (this.m_Process != null)
            {
                try
                {
                    this.m_Process.Kill();
                    this.m_Process = null;
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
        
        private void StartServer()
        {
            var file = Assembly.GetExecutingAssembly().Location;
            this.m_Process = new Process();
            this.m_Process.StartInfo.FileName = file;
            this.m_Process.StartInfo.Arguments = "--server --address 127.0.0.1 --port 9091";
            this.m_Process.EnableRaisingEvents = true;
            this.m_Process.Exited += (sender, e) => 
            {
                Console.WriteLine("server exited");
                if (!this.m_NormalShutdown)
                {
                    throw new InvalidOperationException("server exited, game exiting too");
                }
            };
            this.m_Process.Start();
        }
    }
}
