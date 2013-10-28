// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Net;
using Dx.Runtime;
using Ninject;
using Tychaia.Network;

namespace Tychaia
{
    public class TychaiaServer
    {
        private readonly IDxFactory m_DxFactory;
        private readonly IKernel m_Kernel;
    
        public TychaiaServer(
            IDxFactory dxFactory,
            IKernel kernel)
        {
            this.m_DxFactory = dxFactory;
            this.m_Kernel = kernel;
        }
        
        public void Run(string address, int port)
        {
            TychaiaTCPNetwork.SetupKernel(this.m_Kernel, true, IPAddress.Parse(address), port);
        
            Console.WriteLine("Creating distributed node...");
            var node = this.m_DxFactory.CreateLocalNode(Caching.PushOnChange, Architecture.ServerClient);
            node.Join(null);
        
            // Register the local node with Ninject so that entities and worlds can
            // gain access to it.
            this.m_Kernel.Bind<ILocalNode>().ToMethod(x => node);
            
            // Create the GameState.
            var state = (GameState)new Distributed<GameState>(node, GameState.NAME);
            Console.WriteLine("Server is now running.");
            
            // Run the game.
            while (true)
            {
                state.Update();
            }
        }
    }
}
