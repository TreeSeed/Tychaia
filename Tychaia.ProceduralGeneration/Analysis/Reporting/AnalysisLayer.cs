//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "layer")]
    public class AnalysisLayer
    {
        [DataMember(Name = "name")]
        public string
            Name;

        [DataMember(Name = "code")]
        public string
            Code;
    }
}

