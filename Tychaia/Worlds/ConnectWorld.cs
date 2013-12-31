// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Ninject;
using Protogame;
using Tychaia.Network;

namespace Tychaia
{
    public class ConnectWorld : MenuWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly Action[] m_Actions;

        private readonly IPAddress m_Address;

        private readonly FontAsset m_DefaultFont;

        private readonly Action m_FinalAction;

        private readonly int m_Port;

        private IGameContext m_GameContext;

        private string m_Message = string.Empty;

        private volatile bool m_PerformFinalAction;

        private Process m_Process;

        public ConnectWorld(
            IKernel kernel, 
            I2DRenderUtilities twodRenderUtilities, 
            IAssetManagerProvider assetManagerProvider, 
            IBackgroundCubeEntityFactory backgroundCubeEntityFactory, 
            ISkin skin, 
            bool startServer, 
            IPAddress address, 
            int port)
            : base(twodRenderUtilities, assetManagerProvider, backgroundCubeEntityFactory, skin)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;

            this.m_DefaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
            this.m_Address = address;
            this.m_Port = port;
            TychaiaClient client = null;
            byte[] initial = null;
            Action cleanup = () =>
            {
                kernel.Unbind<INetworkAPI>();
                kernel.Unbind<IClientNetworkAPI>();
                if (client != null)
                {
                    client.Close();
                }

                this.TerminateExistingProcess();
            };

            if (startServer)
            {
                this.m_Actions = new Action[]
                {
                    () => this.m_Message = "Closing old process...", () => this.TerminateExistingProcess(), 
                    () => this.m_Message = "Starting server...", () => this.StartServer(), 
                    () => this.m_Message = "Creating client...", () => client = new TychaiaClient(9090), 
                    () => this.m_Message = "Connecting to server...", 
                    () => client.Connect(new IPEndPoint(address, port)),
                    () => this.m_Message = "Binding node to kernel...", 
                    () => kernel.Bind<INetworkAPI>().ToMethod(x => client), 
                    () => kernel.Bind<IClientNetworkAPI>().ToMethod(x => client), () => this.m_Message = "Joining game...", 
                    () => this.JoinGame(client), () => this.m_Message = "Retrieving initial game state...", 
                    () => initial = client.LoadInitialState(), () => this.m_Message = "Starting client...", 
                    () => this.m_PerformFinalAction = true
                };
            }
            else
            {
                this.m_Actions = new Action[]
                {
                    () => this.m_Message = "Creating client...", () => client = new TychaiaClient(9090), 
                    () => this.m_Message = "Connecting to server...", 
                    () => client.Connect(new IPEndPoint(address, port)),
                    () => this.m_Message = "Binding node to kernel...", 
                    () => kernel.Bind<INetworkAPI>().ToMethod(x => client),
                    () => kernel.Bind<IClientNetworkAPI>().ToMethod(x => client), 
                    () => this.m_Message = "Joining game...", 
                    () => this.JoinGame(client), () => this.m_Message = "Retrieving initial game state...", 
                    () => initial = client.LoadInitialState(), () => this.m_Message = "Starting client...", 
                    () => this.m_PerformFinalAction = true
                };
            }

            this.m_FinalAction =
                () =>
                this.TargetWorld =
                this.GameContext.CreateWorld<IWorldFactory>(x => x.CreateTychaiaGameWorld(initial, cleanup));

            var thread = new Thread(this.Run) { IsBackground = true };
            thread.Start();

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

        public override void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            base.RenderBelow(gameContext, renderContext);

            if (renderContext.Is3DContext)
            {
                return;
            }

            this.m_2DRenderUtilities.RenderText(
                renderContext, 
                new Vector2(400, 400), 
                this.m_Message, 
                this.m_DefaultFont);
        }

        public void Run()
        {
            foreach (var action in this.m_Actions)
            {
                action();
            }
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);

            this.m_GameContext = gameContext;

            if (this.m_PerformFinalAction)
            {
                this.m_FinalAction();
            }
        }

        public void JoinGame(TychaiaClient client)
        {
            var random = new Random();
            var playerID = random.Next();

            var hasJoinedGame = false;

            Console.WriteLine("You are player " + playerID);
            
            client.ListenForMessage(
                "join confirm",
                (mcx, s) =>
                {
                    Console.WriteLine("Informed by server we have joined!");
                    hasJoinedGame = true;
                });

            while (!hasJoinedGame)
            {
                client.SendMessage("join", Encoding.ASCII.GetBytes("player " + playerID));

                client.Update();

                Thread.Sleep(1000 / 30);
            }
        }

        private void StartServer()
        {
            var file = Assembly.GetExecutingAssembly().Location;
            this.m_Process = new Process();
            this.m_Process.StartInfo.FileName = file;
            this.m_Process.StartInfo.Arguments = "--server --address " + this.m_Address + " --port " + this.m_Port;
            this.m_Process.EnableRaisingEvents = true;
            this.m_Process.Exited += (sender, e) =>
            {
                Console.WriteLine("server exited");
                this.m_GameContext.SwitchWorld<TitleWorld>();
            };
            this.m_Process.Start();
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
    }
}