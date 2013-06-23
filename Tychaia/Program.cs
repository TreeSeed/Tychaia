//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Windows.Forms;
using NDesk.Options;
using TychaiaAssetManager;
using System.Diagnostics;
using Process4.Attributes;
using Tychaia.Globals;
using Tychaia.Assets;

namespace Tychaia
{
    [Distributed(Architecture.ServerClient, Caching.PushOnChange)]
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var startAssetManager = false;
            var options = new OptionSet
            {
                { "asset-manager", "Start the asset manager with the game.", v => startAssetManager = true }
            };
            try
            {
                options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.Write("Tychaia.exe: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try `Tychaia.exe --help` for more information.");
                return;
            }

            Process assetManagerProcess = null;
            if (startAssetManager)
            {
                assetManagerProcess = AssetManagerClient.RunAndConnect();
            }
            else
            {
                IoC.Kernel.Bind<IAssetManagerProvider>().To<GameAssetManagerProvider>().InSingletonScope();
            }

            if (assetManagerProcess != null)
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    // Make sure we close down the asset manager process if it's there.
                    if (assetManagerProcess != null)
                    {
                        assetManagerProcess.Kill();
                    }
                };
            }

            using (RuntimeGame game = new RuntimeGame())
            {
                try
                {
                    game.Run();
                }
                catch (EntryPointNotFoundException)
                {
#if PLATFORM_WINDOWS || PLATFORM_LINUX
                    if (!game.HasTicked)
                        MessageBox.Show("Tychaia requires a graphics card with 3D hardware acceleration.", "Sorry!");
                    else
#endif
                        throw;
                }
            }

            if (assetManagerProcess != null)
            {
                assetManagerProcess.Kill();
            }
        }
    }
}

