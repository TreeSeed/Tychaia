// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;

namespace Tychaia.ProceduralGeneration
{
    public class FlowBundle
    {
        public readonly int Count;
        public readonly dynamic[] Data;
        public readonly string[] Name;

        public FlowBundle()
        {
            this.Name = new string[0];
            this.Data = new dynamic[0];
            this.Count = 0;
        }

        private FlowBundle(string[] names, dynamic[] values, int valueCount)
        {
            this.Name = names;
            this.Data = values;
            this.Count = valueCount;
        }

        private FlowBundle Add(string name, dynamic value)
        {
            var dataCopy = new dynamic[this.Count + 1];
            var nameCopy = new string[this.Count + 1];
            this.Data.CopyTo(dataCopy, 0);
            this.Name.CopyTo(nameCopy, 0);
            dataCopy[this.Count] = value;
            nameCopy[this.Count] = name;
            return new FlowBundle(nameCopy, dataCopy, this.Count + 1);
        }

        private int IndexOf(string name)
        {
            for (var i = 0; i < this.Count; i++)
                if (this.Name[i] == name)
                    return i;
            return -1;
        }

        public bool Exists(string name)
        {
            return this.IndexOf(name) != -1;
        }

        public FlowBundle Set(string name, dynamic value)
        {
            if (!this.Exists(name))
                return this.Add(name, value);
            var idx = this.IndexOf(name);
            var nameCopy = this.Name.ToArray();
            var dataCopy = this.Data.ToArray();
            dataCopy[idx] = value;
            return new FlowBundle(nameCopy, dataCopy, this.Count);
        }

        public dynamic Get(string name)
        {
            for (var i = 0; i < this.Count; i++)
                if (this.Name[i] == name)
                    return this.Data[i];
            return null;
        }

        public FlowBundle Delete(string name)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this.Name[i] == name)
                {
                    var dataCopy = new dynamic[this.Count - 1];
                    var nameCopy = new string[this.Count - 1];
                    for (var x = 0; x < i; x++)
                    {
                        dataCopy[x] = this.Data[x];
                        nameCopy[x] = this.Name[x];
                    }
                    for (var x = i + 1; x < this.Count; x++)
                    {
                        dataCopy[x - 1] = this.Data[x];
                        nameCopy[x - 1] = this.Name[x];
                    }
                    return new FlowBundle(nameCopy, dataCopy, this.Count + 1);
                }
            }
            return new FlowBundle(this.Name.ToArray(), this.Data.ToArray(), this.Count);
        }

        public int Hash()
        {
            var result = 0;
            foreach (var blob in this.Data)
            {
                if (blob is int)
                    result += (blob - 73903) * 12927;
                else if (blob is string)
                {
                    for (var a = 0; a < blob.Length; a++)
                        result += (blob[a] + 21299) * 73703;
                }
                else
                    result += blob.GetHashCode() * 21299;
            }
            return result;
        }
    }
}
