// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Flow;
using ManyConsole;
using System.Linq;

namespace TychaiaTool
{
    public class ProceduralTracingCommand : ConsoleCommand
    {
        private readonly IConfigurationHelper m_ConfigurationHelper;
        private readonly IAlgorithmTraceImageGeneration m_AlgorithmTraceImageGeneration;

        private IProceduralConfiguration m_Configuration;
        private string m_ConfigurationName;

        private Action m_EnableHandler;
        private Action m_DisableHandler;
        private Dictionary<string, int> m_SaveNames = new Dictionary<string, int>();
        private int m_Count = 0;

        public ProceduralTracingCommand(
            IConfigurationHelper configurationHelper,
            IAlgorithmTraceImageGeneration algorithmTraceImageGeneration)
        {
            this.m_ConfigurationHelper = configurationHelper;
            this.m_AlgorithmTraceImageGeneration = algorithmTraceImageGeneration;

            this.IsCommand("trace-generation", "Trace the world configuration into a set of images");
            this.m_ConfigurationHelper.Setup(this, x => this.m_ConfigurationName = x);
        }

        private void PerformOperationRecursively(Action<RuntimeLayer> operation, RuntimeLayer layer)
        {
            operation(layer);
            foreach (var input in layer.GetInputs().Where(input => input != null))
                PerformOperationRecursively(operation, input);
        }

        private void PerformOperation(Action<IGenerator> operation, IGenerator generator)
        {
            if (generator is RuntimeLayer)
                this.PerformOperationRecursively(operation, (RuntimeLayer)generator);
            else
                operation(generator);
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

            this.m_EnableHandler = () =>
                this.PerformOperation(x => x.DataGenerated += this.HandleDataGenerated, generator);
            this.m_DisableHandler = () =>
                this.PerformOperation(x => x.DataGenerated -= this.HandleDataGenerated, generator);

            this.m_EnableHandler();

            var s = 64;
            var o = 10000000;
            int computations;
            generator.GenerateData(-s + o, -s + o, -s + o, s * 2, s * 2, s * 2, out computations);

            return 0;
        }

        private void HandleDataGenerated(object sender, DataGeneratedEventArgs e)
        {
            var name = (sender as RuntimeLayer).Userdata as string;
            if (name == null)
                name = sender.GetType().Name;
            if (!m_SaveNames.ContainsKey(name))
                m_SaveNames[name] = 0;
            m_SaveNames[name] += 1;

            // Save the internal result.
            var bitmap = this.m_AlgorithmTraceImageGeneration.RenderTraceResult(
                sender as RuntimeLayer,
                e.Data,
                e.GSArrayWidth,
                e.GSArrayHeight,
                e.GSArrayDepth);
            Console.WriteLine(name + ": " + m_SaveNames[name] + " (internal)");
            bitmap.Save("layer_" + ++m_Count + ".png");

            // Save the normal result.
            int computations;
            this.m_DisableHandler();
            var data = (sender as RuntimeLayer)
                .GenerateData(e.GSAbsoluteX - e.GSMaxOffsetX,
                              e.GSAbsoluteY - e.GSMaxOffsetY,
                              e.GSAbsoluteZ - e.GSMaxOffsetZ,
                              e.GSArrayWidth,
                              e.GSArrayHeight,
                              e.GSArrayDepth, out computations);
            var alt = this.m_AlgorithmTraceImageGeneration.RenderTraceResult(
                    sender as RuntimeLayer,
                    data,
                    e.GSArrayWidth,
                    e.GSArrayHeight,
                    e.GSArrayDepth);
            Console.WriteLine(name + ": " + m_SaveNames[name] + " (normal)");
            alt.Save("layer_" + ++m_Count + ".png");
            this.m_EnableHandler();
        }
    }
}

