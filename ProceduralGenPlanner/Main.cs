// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;
using Ninject;
using System;

namespace ProceduralGenPlanner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load<TychaiaProceduralGenerationIoCModule>();

            var generator = kernel.Get<IGeneratorResolver>().GetGeneratorForGame();

            var planner = kernel.Get<IGenerationPlanner>();
            var request = planner.CreateRequest(generator);
            request.AddRegion(-20, -20, 0, 10, 10, 1);
            request.AddRegion(-10, -10, 0, 20, 10, 1);
            request.AddRegion(0, 0, 0, 10, 10, 1);
            request.AddRegion(0, 5, 0, 10, 30, 1);
            request.AddRegion(100, 100, 100, 10, 10, 1);
            request.AddRegion(200, 200, 200, 50, 50, 1);
            request.AddRegion(300, 200, 200, 50, 50, 1);
            request.Progress += (sender, e) => Console.WriteLine("Progress: " + e.Progress + "%");
            request.RegionComplete += (sender, e) => Console.WriteLine("Region Complete: " + e.Region);
            planner.Execute(request);
        }
    }
}
