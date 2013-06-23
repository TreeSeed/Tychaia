//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Process4;
using Tychaia.Network;
using Tychaia.Globals;
using Tychaia.Assets;
using Ninject;
using NDesk.Options;
using Process4.Attributes;

namespace TychaiaAssetManager
{
    [Distributed(Architecture.ServerClient, Caching.PushOnChange)]
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var connectToRunningGame = false;
            var options = new OptionSet
            {
                { "connect", "Internal use only (used by the Tychaia game client).", v => connectToRunningGame = true }
            };
            try
            {
                options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.Write("TychaiaAssetManager.exe: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try `Tychaia.exe --help` for more information.");
                return;
            }

            if (connectToRunningGame)
            {
                var node = new LocalNode();
                node.Network = new TychaiaAssetManagerNetwork(node, true);
                node.Join();
                var assetManagerProvider = new NetworkedAssetManagerProvider(node);
                IoC.Kernel.Bind<IAssetManagerProvider>().ToMethod(x => assetManagerProvider);
            }
            else
            {
                var assetManagerProvider = new LocalAssetManagerProvider();
                IoC.Kernel.Bind<IAssetManagerProvider>().ToMethod(x => assetManagerProvider);
            }

            using (var game = new AssetManagerGame(
                IoC.Kernel.Get<IAssetManagerProvider>().GetAssetManager(true)))
            {
                game.Run();
            }
        }
    }
}
