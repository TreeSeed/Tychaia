using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.FlowBundle
{
    public struct FlowBundle
    {
        public dynamic[] Data;
        public string[] Name;
        public int Count;

        public FlowBundle(int structsize)
        {
            Data = new dynamic[structsize];
            Name = new string[structsize];
            Count = 0;
        }

        public void AddValue(dynamic value)
        {
            if (Count < Data.Length)
            {
                Data[Count] = value;
                Count++;
            }
        }

        public dynamic ExtractValue(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if(Name[i] == name)
                {
                    dynamic datareturn = Data[i];
                    if (i + 1 < Count)
                    {
                        Data[i] = Data[i + 1];
                        Data[i + 1] = null;
                        Name[i] = Name[i + 1];
                        Name[i + 1] = null;
                    }
                    else
                    {
                        Data[i] = null;
                        Name[i] = null;
                    }

                    return datareturn;
                }
            }

            return null;
        }
    }
}
