// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Data
{
    [ProtoContract]
    public class Cell
    {
        [ProtoMember(1)]
        public string BlockAssetName;
        [ProtoMember(2)]
        public int HeightMap;
    }
}

