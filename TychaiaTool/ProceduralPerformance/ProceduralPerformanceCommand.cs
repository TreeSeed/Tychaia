// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using ManyConsole;
using Tychaia.ProceduralGeneration;
using System;

namespace TychaiaTool
{
    public class ProceduralPerformanceCommand : ConsoleCommand
    {
        private readonly IConfigurationHelper m_ConfigurationHelper;

        private IProceduralConfiguration m_Configuration;
        private string m_ConfigurationName;
        private int m_TestsPerMeasure = 1;
        private int m_GenerationSize = 32;
        private bool m_2DGeneration = false;

        public ProceduralPerformanceCommand(
            IConfigurationHelper configurationHelper)
        {
            this.m_ConfigurationHelper = configurationHelper;

            this.IsCommand("test-performance", "Measure the performance of the procedural generation");
            this.m_ConfigurationHelper.Setup(this, x => this.m_ConfigurationName = x);
            this.HasOption(
                "m|tests-per-measure=",
                "The number of tests to perform between measurements (default: 1)",
                (int x) => this.m_TestsPerMeasure = x);
            this.HasOption(
                "s|size=",
                "The size to generate (default: 32)",
                (int x) => this.m_GenerationSize = x);
            this.HasOption(
                "2d",
                "Only generate in 2D (default: false)",
                x => this.m_2DGeneration = true);
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
            var generator = this.m_Configuration.GetConfiguration();

            for (var x = 0; x < 3000; x++)
            {
                int computations;
                var start = DateTime.Now;
                for (var i = 0; i < this.m_TestsPerMeasure; i++)
                    generator.GenerateData(
                        0,
                        0,
                        0,
                        this.m_GenerationSize,
                        this.m_GenerationSize,
                        this.m_2DGeneration ? 1 : this.m_GenerationSize,
                        out computations);
                var end = DateTime.Now;

                Console.Write("Test #{0,4}: ", x);
                Console.Write((end - start).TotalMilliseconds / this.m_TestsPerMeasure + "ms");
                Console.WriteLine();
            }

            return 0;
        }
    }
}

