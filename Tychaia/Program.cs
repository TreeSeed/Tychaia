// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ninject;
using Protogame;
using Tychaia.Asset;
using Tychaia.Globals;
using Tychaia.Network;
using Tychaia.ProceduralGeneration;
using Tychaia.Runtime;

namespace Tychaia
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                Run(args);
            }
            else
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, e) => CrashReport.CrashReporter.Record((Exception)e.ExceptionObject);

                try
                {
                    Run(args);
                }
                catch (Exception e)
                {
                    CrashReport.CrashReporter.Record(e);
                }
            }
        }

        private static void Run(string[] args)
        {
            // Fixes T266.
            var directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            Environment.CurrentDirectory = directory.FullName;

            var isServer = false;
            var address = string.Empty;
            var port = 0;
            var options = new[]
            {
                new ExtraOption
                {
                    Prototype = "server",
                    Description = "Whether Tychaia should run as a console-only server.",
                    Action = x => isServer = true
                },
                new ExtraOption
                {
                    Prototype = "address=",
                    Description = "The TCP IP address to listen on when running as a server.",
                    Action = x => address = x
                },
                new ExtraOption
                {
                    Prototype = "port=",
                    Description = "The TCP port to listen on when running as a server.",
                    Action = x => port = Convert.ToInt32(x)
                }
            };
        
            // Set up basic kernel modules.
            var kernel = new StandardKernel();
            kernel.Load<Protogame3DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            kernel.Load<ProtogameEventsIoCModule>();
            kernel.Load<ProtogameCachingIoCModule>();
            kernel.Load<TychaiaIoCModule>();
            kernel.Load<TychaiaAssetIoCModule>();
            kernel.Load<TychaiaIsometricIoCModule>();
            kernel.Load<TychaiaGlobalIoCModule>();
            kernel.Load<TychaiaNetworkIoCModule>();
            kernel.Load<TychaiaRuntimeIoCModule>();
            
            // Modules after this point require IPersistentStorage, so we need to parse our command line
            // and then load the server module if needed, to rebind previous bindings.
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, args, options);
            if (isServer)
                kernel.Load<TychaiaServerIoCModule>();
                
            // Load somewhat more advanced modules that may depend on services registered previously.
            kernel.Load<TychaiaProfilingIoCModule>();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();

            if (isServer)
            {
                var server = kernel.Get<TychaiaServerRunner>();
                server.Run(address, port);

                Thread.Sleep(10000000);
                return;
            }

#if PLATFORM_LINUX
            // TODO: Check if the following libraries are present:
            // * libXi (sudo ln -s /usr/lib64/libXi.so.6 /usr/lib/libXi.so.6)

            // If we are running Linux, we need to mark the Tychaia.exe file as executable.
            // This allows the server to run as a seperate process.
            try
            {
                var p = Process.Start("chmod", "u+x '" + Assembly.GetExecutingAssembly().Location + "'");
                p.WaitForExit();
            }
            catch
            {
                Console.WriteLine("WARNING: Unable to mark Tychaia.exe as executable.  You may have trouble running the game!");
            }
#endif

            using (var game = new TychaiaGame(kernel))
            {
                try
                {
                    game.Run();
                }
                catch (EntryPointNotFoundException)
                {
#if PLATFORM_WINDOWS || PLATFORM_LINUX
                    MessageBox.Show("Tychaia requires a graphics card with 3D hardware acceleration.", "Sorry!");
#else
                    throw;
#endif
                }
            }
        }
    }
}
