using ProtoBuf;

namespace Tychaia.Network
{
    [ProtoContract]
    public class PlayerList
    {
        [ProtoMember(1)]
        public string[] Players { get; set; }
    }
}
