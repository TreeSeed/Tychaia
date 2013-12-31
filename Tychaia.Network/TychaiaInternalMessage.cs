// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Network
{
    [ProtoContract]
    public class TychaiaInternalMessage
    {
        [ProtoMember(2)]
        public byte[] Data { get; set; }

        [ProtoMember(1)]
        public string Type { get; set; }
    }
}