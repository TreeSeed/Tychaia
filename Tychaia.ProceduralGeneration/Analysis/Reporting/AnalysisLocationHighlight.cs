//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "highlight")]
    public class AnalysisLocationHighlight : AnalysisLocation
    {
        [DataMember(Name = "importance")]
        public int
            Importance;

        [DataMember(Name = "message")]
        public string
            Message;
    }
}

