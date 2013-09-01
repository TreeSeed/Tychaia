// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using ManyConsole;
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class ProceduralPlannerCommand : ConsoleCommand
    {
        private readonly IConfigurationHelper m_ConfigurationHelper;
        private readonly IGenerationPlanner m_GenerationPlanner;

        private IProceduralConfiguration m_Configuration;
        private string m_ConfigurationName;

        public ProceduralPlannerCommand(
            IConfigurationHelper configurationHelper,
            IGenerationPlanner generationPlanner)
        {
            this.m_ConfigurationHelper = configurationHelper;
            this.m_GenerationPlanner = generationPlanner;

            this.IsCommand("test-planner", "Test the procedural generation planning engine");
            this.m_ConfigurationHelper.Setup(this, x => this.m_ConfigurationName = x);
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            this.m_Configuration = this.m_ConfigurationHelper.Validate(this.m_ConfigurationName);
            return this.m_Configuration == null
                ? 1
                : base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }

        public override int Run(string[] remainingArguments)
        {
            var request = this.m_GenerationPlanner.CreateRequest(
                this.m_Configuration.GetConfiguration());
            request.AddRegion(-20, -20, 0, 10, 10, 1);
            request.AddRegion(-10, -10, 0, 20, 10, 1);
            request.AddRegion(0, 0, 0, 10, 10, 1);
            request.AddRegion(0, 5, 0, 10, 30, 1);
            request.AddRegion(100, 100, 100, 10, 10, 1);
            request.AddRegion(200, 200, 200, 50, 50, 1);
            request.AddRegion(300, 200, 200, 50, 50, 1);
            request.Progress += (sender, e) => Console.WriteLine("Progress: " + e.Progress + "%");
            request.RegionComplete += (sender, e) => Console.WriteLine("Region Complete: " + e.Region);
            this.m_GenerationPlanner.Execute(request);

            return 0;
        }
    }
}

