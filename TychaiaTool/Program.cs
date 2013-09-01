// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Ninject;
using Tychaia.Globals;
using Tychaia.ProceduralGeneration;
using ManyConsole;

namespace TychaiaTool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load<TychaiaGlobalIoCModule>();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();
            kernel.Load<TychaiaToolIoCModule>();
            ConsoleCommandDispatcher.DispatchCommand(
                kernel.GetAll<ConsoleCommand>(),
                args,
                Console.Out);
        }
    }
}

