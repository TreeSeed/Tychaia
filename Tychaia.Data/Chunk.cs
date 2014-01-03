// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Data
{
    [ProtoContract]
    public class Chunk
    {
        [ProtoMember(4)]
        public Cell[] Cells;

        [ProtoMember(5)]
        public int[] Indexes;

        [ProtoMember(6)]
        public Vertex[] Vertexes;

        [ProtoMember(1)]
        public long X;

        [ProtoMember(2)]
        public long Y;

        [ProtoMember(3)]
        public long Z;

        [ProtoMember(7)]
        public bool Generated;
    }
}