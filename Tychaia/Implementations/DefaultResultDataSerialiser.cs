// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using Tychaia.ProceduralGeneration;

namespace Tychaia
{
    public class DefaultResultDataSerialiser : IResultDataSerialiser
    {
        public void Serialise(BinaryWriter writer, ResultData resultData)
        {
            writer.Write(resultData.HeightMap);
            if (resultData.BlockInfo.BlockAssetName == null)
                writer.Write(false);
            else
            {
                writer.Write(true);
                writer.Write(resultData.BlockInfo.BlockAssetName);
            }
        }

        public ResultData Deserialise(BinaryReader reader)
        {
            var result = new ResultData();
            result.HeightMap = reader.ReadInt32();
            result.BlockInfo = new BlockInfo();
            if (reader.ReadBoolean())
                result.BlockInfo.BlockAssetName = reader.ReadString();
            else
                result.BlockInfo.BlockAssetName = null;
            return result;
        }
    }
}

