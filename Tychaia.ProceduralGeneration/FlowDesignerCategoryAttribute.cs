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
        // These are general tools
        General,
        Initials,
        Zooming,
        Manipulation,
        Debugging,
        Output,
        // Want to have a seperate heading for anything more specific catergories
        Land,
        Biome, // Required
        Rainfall,
        Temperature,
        Rivers,
        Trees,
        Towns,
        FamilyTrees // Haven't thought this thru layer wise yet
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

