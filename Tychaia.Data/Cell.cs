// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ProtoBuf;

namespace Tychaia.Data
{
    [ProtoContract]
    public struct Cell
    {
        // Land Generation
        [ProtoMember(1)]
        public string BlockAssetName;
        [ProtoMember(2)]
        public int HeightMap;
        [ProtoMember(15)]
        public int EdgeDetection;

        // Being Clusters Generation
        [ProtoMember(3)]
        public string ClusterDefinitionAssetName;
        [ProtoMember(4)]
        public int Count0;
        [ProtoMember(5)]
        public int Count1;
        [ProtoMember(6)]
        public int Count2;
        [ProtoMember(7)]
        public int Count3;
        [ProtoMember(8)]
        public int Count4;
        [ProtoMember(9)]
        public int Count5;
        [ProtoMember(10)]
        public int Count6;
        [ProtoMember(11)]
        public int Count7;
        [ProtoMember(12)]
        public int Count8;
        [ProtoMember(13)]
        public int Count9;
        [ProtoMember(14)]
        public bool ClusterComplete;

        // Beings Generation
        [ProtoMember(15)]
        public int BeingHealth;
    }
}
