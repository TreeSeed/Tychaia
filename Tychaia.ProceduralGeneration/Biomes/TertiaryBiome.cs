using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.ProceduralGeneration.Biomes
{
    public abstract class TertiaryBiome
    {
        public Type[] SuitableSecondaryBiomes;
        public Type[] RequiredBuildings;
    }
}
