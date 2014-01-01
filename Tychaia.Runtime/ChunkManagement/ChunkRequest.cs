// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Runtime
{
    [ProtoContract]
    public class ChunkRequest
    {
        [ProtoMember(1)]
        public long X { get; set; }

        [ProtoMember(2)]
        public long Y { get; set; }

        [ProtoMember(3)]
        public long Z { get; set; }
    }
}