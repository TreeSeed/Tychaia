//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "issue")]
    public class AnalysisIssue
    {
        [DataMember(Name = "layer")]
        public AnalysisLayer
            Layer;

        [DataMember(Name = "id")]
        public string
            ID;

        [DataMember(Name = "name")]
        public string
            Name;

        [DataMember(Name = "description")]
        public string
            Description;

        [DataMember(Name = "locations")]
        public List<AnalysisLocation>
            Locations = new List<AnalysisLocation>();
    }
}

