// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "analysis")]
    public class Analysis
    {
        [DataMember(Name = "layers")] public List<AnalysisLayer>
            Layers = new List<AnalysisLayer>();
    }
}