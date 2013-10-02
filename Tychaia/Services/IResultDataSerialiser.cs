// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    public interface IResultDataSerialiser
    {
        void Serialise(BinaryWriter writer, ResultData resultData);
        ResultData Deserialise(BinaryReader reader);
    }
}
