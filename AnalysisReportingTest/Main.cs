//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.ProceduralGeneration.Analysis.Reporting;
using System.IO;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Compiler;
using ICSharpCode.Decompiler.Ast;

namespace AnalysisReportingTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            AstBuilder astBuilder;
            var algorithm = new AlgorithmInitialBool();
            var method = DecompileUtil.GetAlgorithmCode(algorithm.GetType(), out astBuilder);

            var layer = new AnalysisLayer();
            layer.Name = algorithm.GetType().FullName;
            layer.Code = method.GetText();
            var report = new AnalysisReport();
            report.Name = "Simplification Analysis";
            var issue = new AnalysisIssue();
            issue.ID = "S0001";
            issue.Name = "Replace Expressions";
            issue.Layer = layer;
            issue.Description = "Replace the expressions dawg.";
            issue.Locations.Add(new AnalysisLocationHighlight { Start = 10, End = 20, Importance = 50, Message = "test" });
            report.Issues.Add(issue);
            var analysis = new Analysis();
            analysis.Layers.Add(layer);
            analysis.Reports.Add(report);

            using (var writer = new StreamWriter("test.xml"))
            {
                AnalysisIO.Save(analysis, writer);
            }
        }
    }
}
