// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Net;
using System.Threading;
using Dx.Runtime;
using Ninject;
using Phabricator.Conduit;
using Tychaia.Network;

namespace Tychaia
{
    public class TychaiaServer
    {
        private readonly IKernel m_Kernel;

        public TychaiaServer(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public void Run(string address, int port)
        {
            Console.WriteLine("Creating distributed node on " + address + ":" + port + "...");
            var node = new LocalNode(Architecture.ServerClient, Caching.PushOnChange) { IsServer = true };
            node.Bind(IPAddress.Parse(address), port);

            // Register the local node with Ninject so that entities and worlds can
            // gain access to it.
            this.m_Kernel.Bind<ILocalNode>().ToMethod(x => node);

            // Create the GameState.
            var state = (GameState)new Distributed<GameState>(node, GameState.NAME);
            Console.WriteLine("Server is now running.");

            var start = DateTime.Now;
            var spinWait = new SpinWait();

            var pingThread = new Thread(() => this.PingServerList(IPAddress.Parse(address), port))
            {
                IsBackground = true
            };
            pingThread.Start();

            // Run the game.
            var step = 0L;
            while (true)
            {
                state.Update();

                while (this.CalculateTotalFrames(start) <= step)
                {
                    spinWait.SpinOnce();
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private long CalculateTotalFrames(DateTime start)
        {
            var ms = (DateTime.Now - start).TotalMilliseconds;
            const double FRAME_MS = 1000.0 / 60.0;

            return (long)Math.Floor(ms / FRAME_MS);
        }

        private void PingServerList(IPAddress address, int port)
        {
            var last = DateTime.Now;
            var client = new ConduitClient("https://code.redpointsoftware.com.au/api")
            {
                User = "game-server", 
                Certificate =
                    "qyr2axzb2mwuc2vpq74zfebmjxitexas3ril4fhxr3lhq5ytg6p"
                    + "zt4abt6sxckxqjucszq5kijd3ju2pfuthfmerj6r37dokbwwmtk"
                    + "wrlldj3k6uklauf7pandjnkk6zutmohqpsxo3sbopj7wuurkzka"
                    + "42ewwds7zqyzje5ic4mt6upwgo4nj6bse2clfwe73xxhhnjmwpg"
                    + "zqxqgm6lyqv45evpbjmfsdw7cnz4jmhmkij75efjvzfma4eeuul"
            };

            while (true)
            {
                Console.WriteLine("Pinging server list...");

                client.Do("serverlist.ping", new { name = "game server", host = address.ToString(), port });

                var wait = (last.AddSeconds(60) - DateTime.Now).TotalSeconds;
                if (wait > 0)
                {
                    Thread.Sleep((int)(wait * 1000));
                }

                last = DateTime.Now;
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}