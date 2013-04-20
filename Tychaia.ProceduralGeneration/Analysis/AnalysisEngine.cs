//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.ProceduralGeneration.Analysis.Reporting;
using ICSharpCode.Decompiler.Ast;

namespace Tychaia.ProceduralGeneration.Analysis
{
    public abstract class AnalysisEngine
    {
        public abstract void Process(AnalysisLayer layer, ref Analysis.Reporting.Analysis analysis);
    }
}

