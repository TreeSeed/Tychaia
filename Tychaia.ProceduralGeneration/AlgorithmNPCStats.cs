//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Tychaia.ProceduralGeneration.Biomes;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.Specific)]
    [FlowDesignerCategory(FlowCategory.NPCs)]
    [FlowDesignerName("NPC Stats")]
    public class AlgorithmNPCStats : Algorithm<int, int>
    {        
        [DataMember]
        [DefaultValue(Stat.Trust)]
        [Description("Which stat are you modifying?")]
        public Stat StatSelected
        {
            get;
            set;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Profession" }; }
        }

        public override bool Is2DOnly
        {
            get { return true; }
        }

        public AlgorithmNPCStats()
        {
            this.StatSelected = Stat.Trust;
        }

        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            // IF Input has favored value then take 10% towards that value, else take random 0 - 100.
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            int a = (int)(value * 2.55);
         
            if (a < 0)
                a = 0;
            if (a > 255)
                a = 255;

            return Color.FromArgb(a, a, a);
        }

        public enum Stat
        {                                   // For each value certain professions will favor near certain values
            Trust,                          // How much the NPC trusts people
            LocalReputation,                // How much the local trusts this person
            RegionReputation,               // How much the region trusts this person 
            SelfishGiving,                  // If this person cares more for themself or others
            GoodEvil,                       // If this person is generall good or evil
            SimpleComplex,                  // If this person thinks of the bigger picture or for the quick solution
            OptimisticPessimistic,          // How this person is effected by events, if a negative event affects an optimistic person they may not have as many problems
            AdventurousCautious             // If this person favors going out and doing the task themself
        }
    }

}

