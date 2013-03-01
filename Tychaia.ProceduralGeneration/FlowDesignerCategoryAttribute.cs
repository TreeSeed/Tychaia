//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.ProceduralGeneration
{
    // Going to have to re-think categories - currently in the process of making layers more dynamic (so we can use them for multiple things that are simmilar)
    public enum FlowCategory
    {
        General,
        BaseLayers,
        ZoomTools,
        Land,
        Biome,
        Rainfall,
        Temperature,
        Rivers,
        Trees,
        Towns,
        FamilyTrees,
        Debugging
    }

    public class FlowDesignerCategoryAttribute : Attribute
    {
        public FlowCategory Category
        {
            get;
            private set;
        }
        
        public FlowDesignerCategoryAttribute(FlowCategory category)
        {
            this.Category = category;
        }
    }
}

