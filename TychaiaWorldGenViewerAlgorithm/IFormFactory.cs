// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Redpoint.FlowGraph;

namespace TychaiaWorldGenViewerAlgorithm
{
    public interface IFormFactory
    {
        AnalyseForm CreateAnalyseForm(FlowElement flowElement);
        ExportForm CreateExportForm(FlowElement flowElement);
        TraceForm CreateTraceForm(FlowElement flowElement);
    }
}
