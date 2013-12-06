// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Windows.Forms;
using Ninject;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;

namespace TychaiaWorldGenViewerAlgorithm
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var kernel = new StandardKernel();
            kernel.Load<TychaiaGlobalIoCModule>();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();
            kernel.Load<TychaiaWorldGenViewerAlgorithmIoCModule>();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(kernel.Get<FlowForm>());
        }
    }
}
