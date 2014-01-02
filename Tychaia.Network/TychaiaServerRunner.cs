// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Net;
using System.Threading;
using Ninject;
using Phabricator.Conduit;
using Protogame;

namespace Tychaia.Network
{
    public class TychaiaServerRunner
    {
        private readonly IKernel m_Kernel;

        public TychaiaServerRunner(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public void Run(string address, int port)
        {
            Console.WriteLine("Creating server on " + port + "...");
            var server = new TychaiaServer(port, port + 1);
            server.StartWorld(this.m_Kernel);

            // Register the local node with Ninject so that entities and worlds can
            // gain access to it.
            this.m_Kernel.Bind<INetworkAPI>().ToMethod(x => server);

            // Create the GameState.
            Console.WriteLine("Server is now running.");

            var pingThread = new Thread(() => this.PingServerList(IPAddress.Parse(address), port))
            {
                IsBackground = true
            };
            pingThread.Start();

            // Run the game.
            while (true)
            {
                var start = DateTime.Now;
                server.Update();

                var amount = (1000 / 30) - (int)(DateTime.Now - start).TotalMilliseconds;
                if (amount > 0)
                {
                    Thread.Sleep(amount);
                }
                else
                {
                    Console.WriteLine("WARNING: Tick took " + (int)(DateTime.Now - start).TotalMilliseconds + "ms, which is longer than 33ms.");
                }
            }

            // ReSharper disable once FunctionNeverReturns
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