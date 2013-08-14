// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration.Analysis.Reporting
{
    [DataContract(Name = "location")]
    public abstract class AnalysisLocation
    {
        [DataMember(Name = "end")] public int
            End;

        [DataMember(Name = "start")] public int
            Start;

        [DataMember(Name = "uniqueid")] public int
            UniqueID;

        [DataMember(Name = "uniqueidrefs")] public string
            UniqueIDRefs;

        public abstract AnalysisLocation Copy();
    }
}
