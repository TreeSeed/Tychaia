// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Data
{
    [ProtoContract]
    public class InitialGameState
    {
        [ProtoMember(1)]
        public int Seed;
        [ProtoMember(2)]
        public string[] EntityNames;
        [ProtoMember(3)]
        public string[] EntityTypes;
    }
}
