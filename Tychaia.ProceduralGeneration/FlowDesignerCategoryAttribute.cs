//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.ProceduralGeneration
{
    public enum FlowCategory
    {
        General,
        Land,
        Biome,
        Rainfall,
        Temperature,
        Rivers,
        Trees,
        Towns,
        FamilyTrees
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

