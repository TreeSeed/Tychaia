//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.ProceduralGeneration.Analysis.Reporting;
using System.IO;
using System.Linq;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Analysis;
using System;
using System.Collections.Generic;

namespace AnalysisReportingTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var engine = new SimplificationAnalysisEngine();
            var analysis = new Analysis();

            foreach (var layer in GetAllLayersForStaticAnalysis())
            {
                engine.Process(layer, ref analysis);
                if (layer.Reports.Count > 0)
                    analysis.Layers.Add(layer);
            }

            using (var writer = new StreamWriter("test.xml"))
            {
                AnalysisIO.Save(analysis, writer);
            }
        }

        private static IEnumerable<AnalysisLayer> GetAllLayersForStaticAnalysis()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where !type.IsAbstract && typeof(IAlgorithm).IsAssignableFrom(type) && !type.IsGenericType
                let algorithm = Activator.CreateInstance(type) as IAlgorithm
                select new AnalysisLayer(new StorageLayer { Algorithm = algorithm });
        }
    }
}
