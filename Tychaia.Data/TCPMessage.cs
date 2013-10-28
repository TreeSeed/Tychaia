// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Data
{
    [ProtoContract]
    public class TCPMessage
    {
        public const int ModeUnset = 0;
        public const int ModeError = 1;
        public const int ModeMessageJoin = 2;
        public const int ModeMessageAccept = 3;
    
        [ProtoMember(1)]
        public int Length;
    
        [ProtoMember(2)]
        public int Mode;
        
        [ProtoMember(3)]
        public int Port;
        
        [ProtoMember(4)]
        public byte[] ID;
    }
}
