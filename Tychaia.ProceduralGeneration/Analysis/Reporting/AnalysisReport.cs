//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "report")]
    public class AnalysisReport
    {
        [DataMember(Name = "name")]
        public string
            Name;

        [DataMember(Name = "issues")]
        public List<AnalysisIssue>
            Issues = new List<AnalysisIssue>();
    }
}

