// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.IO;
using ProtoBuf;

namespace Tychaia.Network
{
    public static class InMemorySerializer
    {
        public static T Deserialize<T>(byte[] bytes)
        {
            using (var memory = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(memory);
            }
        }

        public static byte[] Serialize<T>(T instance)
        {
            using (var memory = new MemoryStream())
            {
                Serializer.Serialize(memory, instance);
                var length = (int)memory.Position;
                memory.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[length];
                memory.Read(bytes, 0, length);

                return bytes;
            }
        }
    }
}