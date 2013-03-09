//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Tychaia.ProceduralGeneration.Biomes;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerCategory(FlowCategory.Land)]
    [FlowDesignerName("Terrain To 3D")]
    public class AlgorithmTerrainTo3D : Algorithm<Biome, int, Biome>
    {
        public override string[] InputNames
        {
            get { return new string[] { "Biomes", "Terrain" }; }
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }
        
        public override void Initialize(IRuntimeContext context)
        {
        }

        public override void ProcessCell(IRuntimeContext context, Biome[] inputB, int[] inputA, Biome[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz, int[] ocx, int[] ocy, int[] ocz)
        {
            if (inputA[(i + ox) + (j + oy) * width + (0 + oz) * width * height] > z) 
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = inputB[(i + ox) + (j + oy) * width + (0 + oz) * width * height];
            else if (z == 0)
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = inputB[(i + ox) + (j + oy) * width + (0 + oz) * width * height];
            else
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = null;
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value, 1);
        }
    }

}

