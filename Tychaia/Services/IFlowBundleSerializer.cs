// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    public interface IFlowBundleSerializer
    {
        void Serialize(BinaryWriter writer, FlowBundle bundle);
        FlowBundle Deserialize(BinaryReader reader);
    }
}

