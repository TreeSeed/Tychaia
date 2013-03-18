//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Zooming)]
    [FlowDesignerName("Extend Value")]
    public class AlgorithmExtend : Algorithm<int, int>
    {
        [DataMember]
        [DefaultValue(0)]
        [Description("The value that will be excluded or selected.")]
        public int Value
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("If true extend extend all other values into given value. If false extend all values into anything except for the given value.")]
        public bool ExcludeValue
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("Utilized the neighbors values to provide a better output.")]
        public bool NeighborChancing
        {
            get;
            set;
        }

        public override int[] RequiredXBorder { get { return new int[] {1}; } }
        public override int[] RequiredYBorder { get { return new int[] {1}; } }

        public AlgorithmExtend()
        {
            this.Value = 0;
            this.ExcludeValue = true;
            this.NeighborChancing = false;
        }

        public override string[] InputNames
        {
            get { return new string[] { "Input" }; }
        }
        
        public override bool Is2DOnly
        {
            get { return true; }
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of ExtendLand
        // Towns - This is the equivelent of ExtendTowns
        // Landmarks/Terrain - We can spread landmarks over the world, we can create mountain landmarks by spreading their size out then modifying the terrain.
        // Monsters - We can spread monster villages out, simmilar to towns. 
        // Dungeons - We can use this to smooth dungeons out, so that there are less or more pointy bits. Alternatively we can also use this to spread out some side sections.
        // Anything else?
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            // Does this if statement work?
            // Nevermind have to remake this entire section anyway, too many || && and its getting confusing and over extended.
            // Wondering exactly how to do that - something about having setting checks at the start (then setting booleans) I think, then utilize that throughtout (rather than having multiple checks).
            // Also any other features you can think of?
            if ((input[(i + ox) + (j + oy) * width + (k + oz) * width * height] == Value && ExcludeValue) || (input[(i + ox) + (j + oy) * width + (k + oz) * width * height] != Value && ExcludeValue == false))
            {
                int checkvalue = 50;

                for (int a = 0; a < 1; a++)
                {
                    int selected = context.GetRandomRange(x, y, 0, 8, context.Modifier + checkvalue);
                
                    switch (selected)
                    {
                        case 0:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) + 1) + ((j + oy) + 1) * width + (k + oz) * width * height];
                            break;
                        case 1:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) - 1) + ((j + oy) - 1) * width + (k + oz) * width * height];
                            break;
                        case 2:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) - 1) + ((j + oy) + 1) * width + (k + oz) * width * height]; 
                            break;
                        case 3:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) + 1) + ((j + oy) - 1) * width + (k + oz) * width * height];
                            break;
                        case 4:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) + 0) + ((j + oy) + 1) * width + (k + oz) * width * height];
                            break;
                        case 5:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) + 1) + ((j + oy) + 0) * width + (k + oz) * width * height];
                            break;
                        case 6:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) + 0) + ((j + oy) - 1) * width + (k + oz) * width * height];
                            break;
                        case 7:
                            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[((i + ox) - 1) + ((j + oy) + 0) * width + (k + oz) * width * height];
                            break;
                    }

                    if (this.NeighborChancing && checkvalue < (input[((i + ox) + 1) + ((j + oy) + 1) * width + (k + oz) * width * height]
                        + input[((i + ox) - 1) + ((j + oy) - 1) * width + (k + oz) * width * height] 
                        + input[((i + ox) - 1) + ((j + oy) + 1) * width + (k + oz) * width * height] 
                        + input[((i + ox) + 1) + ((j + oy) - 1) * width + (k + oz) * width * height] 
                        + input[((i + ox) + 0) + ((j + oy) - 1) * width + (k + oz) * width * height] 
                        + input[((i + ox) + 1) + ((j + oy) - 0) * width + (k + oz) * width * height] 
                        + input[((i + ox) + 0) + ((j + oy) + 1) * width + (k + oz) * width * height] 
                        + input[((i + ox) - 1) + ((j + oy) - 0) * width + (k + oz) * width * height]))
                    if ((output[(i + ox) + (j + oy) * width + (k + oz) * width * height] == Value && ExcludeValue) || (output[(i + ox) + (j + oy) * width + (k + oz) * width * height] != Value && ExcludeValue))
                    {
                        checkvalue += 50;
                        a--;
                    }
                }
            }
            else 
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = input[(i + ox) + (j + oy) * width + (k + oz) * width * height];
        }
        
        public override System.Drawing.Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return this.DelegateColorForValueToParent(parent, value);
        }
    }
}

