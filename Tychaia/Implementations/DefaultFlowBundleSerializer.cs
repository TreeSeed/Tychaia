// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using System.Linq;
using Tychaia.ProceduralGeneration;
using System.Collections.Generic;

namespace Tychaia
{
    public class DefaultFlowBundleSerializer : IFlowBundleSerializer
    {
        private Dictionary<byte, string> m_TypeDictionary = new Dictionary<byte, string>
        {
            { 0, typeof(bool).FullName },
            { 1, typeof(int).FullName },
            { 2, typeof(long).FullName },
            { 3, typeof(sbyte).FullName },
            { 4, typeof(float).FullName },
            { 5, typeof(string).FullName },
            { 6, typeof(ushort).FullName },
            { 7, typeof(uint).FullName },
            { 8, typeof(short).FullName },
            { 9, typeof(byte).FullName },
            { 10, typeof(char).FullName },
            { 11, typeof(decimal).FullName },
            { 12, typeof(double).FullName },
            { 13, typeof(ulong).FullName },
            { 14, typeof(BlockInfo).FullName }
        };

        private byte GetTypeIDByValue(object value)
        {
            var typeID = (from kv in this.m_TypeDictionary
                          where kv.Value == value.GetType().FullName
                          select new Nullable<byte>(kv.Key)).FirstOrDefault();
            if (typeID == null)
                return byte.MaxValue;
            return typeID.Value;
        }

        public void Serialize(BinaryWriter writer, FlowBundle bundle)
        {
            writer.Write(bundle.Count);
            for (var i = 0; i < bundle.Count; i++)
                writer.Write(bundle.Name[i]);
            for (var i = 0; i < bundle.Count; i++)
            {
                var value = bundle.Data[i];
                var typeID = this.GetTypeIDByValue(value);
                if (typeID == byte.MaxValue)
                    throw new InvalidOperationException("Can't serialize a flow bundle containing unknown data.");
                writer.Write(typeID);
                switch ((string)value.GetType().FullName)
                {
                    case "Tychaia.ProceduralGeneration.Blocks.BlockInfo":
                        if (value.BlockAssetName == null)
                            writer.Write(false);
                        else
                        {
                            writer.Write(true);
                            writer.Write(value.BlockAssetName);
                        }
                        break;
                    default:
                        writer.Write(value);
                        break;
                }
            }
        }

        public FlowBundle Deserialize(BinaryReader reader)
        {
            var bundle = new FlowBundle();
            var count = reader.ReadInt32();
            var names = new List<string>();
            for (var i = 0; i < count; i++)
                names.Add(reader.ReadString());
            for (var i = 0; i < count; i++)
            {
                var typeID = reader.ReadByte();
                switch (this.m_TypeDictionary[typeID])
                {
                    case "System.Boolean":
                        bundle = bundle.Set(names[i], reader.ReadBoolean());
                        break;
                    case "System.Int32":
                        bundle = bundle.Set(names[i], reader.ReadInt32());
                        break;
                    case "System.Int64":
                        bundle = bundle.Set(names[i], reader.ReadInt64());
                        break;
                    case "System.SByte":
                        bundle = bundle.Set(names[i], reader.ReadSByte());
                        break;
                    case "System.Single":
                        bundle = bundle.Set(names[i], reader.ReadSingle());
                        break;
                    case "System.String":
                        bundle = bundle.Set(names[i], reader.ReadString());
                        break;
                    case "System.UInt16":
                        bundle = bundle.Set(names[i], reader.ReadUInt16());
                        break;
                    case "System.UInt32":
                        bundle = bundle.Set(names[i], reader.ReadUInt32());
                        break;
                    case "System.Int16":
                        bundle = bundle.Set(names[i], reader.ReadInt16());
                        break;
                    case "System.Byte":
                        bundle = bundle.Set(names[i], reader.ReadByte());
                        break;
                    case "System.Char":
                        bundle = bundle.Set(names[i], reader.ReadChar());
                        break;
                    case "System.Decimal":
                        bundle = bundle.Set(names[i], reader.ReadDecimal());
                        break;
                    case "System.Double":
                        bundle = bundle.Set(names[i], reader.ReadDouble());
                        break;
                    case "System.UInt64":
                        bundle = bundle.Set(names[i], reader.ReadUInt64());
                        break;
                    case "Tychaia.ProceduralGeneration.Blocks.BlockInfo":
                        bundle = bundle.Set(names[i], new BlockInfo(reader.ReadBoolean() ? reader.ReadString() : null));
                        break;
                    default:
                        throw new InvalidOperationException("Can't deserialize unknown type.");
                }
            }
            return bundle;
        }
    }
}

