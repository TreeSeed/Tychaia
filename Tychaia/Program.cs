// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Windows.Forms;
using Ninject;
using Process4.Attributes;
using Protogame;
using ProtogameAssetManager;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;
using System.Diagnostics;
using System.Linq;

namespace Tychaia
{
    [Distributed(Architecture.ServerClient, Caching.PushOnChange)]
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame3DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            kernel.Load<ProtogameCachingIoCModule>();
            kernel.Load<TychaiaIoCModule>();
            kernel.Load<TychaiaAssetIoCModule>();
            kernel.Load<TychaiaIsometricIoCModule>();
            kernel.Load<TychaiaGlobalIoCModule>();
            kernel.Load<TychaiaDiskIoCModule>();
            kernel.Load<TychaiaProfilingIoCModule>();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();
            AssetManagerClient.AcceptArgumentsAndSetup<LocalAssetManagerProvider>(kernel, args);

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
