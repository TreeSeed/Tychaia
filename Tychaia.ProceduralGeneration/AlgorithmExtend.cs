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
    [FlowDesignerCategory(FlowCategory.ZoomTools)]
    [FlowDesignerName("Extend")]
    public class AlgorithmExtend : Algorithm<int>
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
        [Description("If true extend all except for the given value else extend only the given value.")]
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

        [DataMember]
        [DefaultValue(false)]
        [Description("When setting this cell also sets neighbor cells. Makes each iteration expand more.")]
        public bool NeighborExpanding
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(false)]
        [Description("Can choose to make the cells only expand when there are less neighbors. Can be either NeighborChancing or defaults to 1 neighbor.")]
        public bool AddPoints
        {
            get;
            set;
        }

        public override int RequiredXBorder { get { return 1; } }
        public override int RequiredYBorder { get { return 1; } }

        public AlgorithmExtend()
        {
            this.Value = 0;
            this.ExcludeValue = true;
            this.NeighborChancing = true;
            this.NeighborExpanding = false;
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of ExtendLand
        // Towns - This is the equivelent of ExtendTowns
        // Landmarks/Terrain - We can spread landmarks over the world, we can create mountain landmarks by spreading their size out then modifying the terrain.
        // Monsters - We can spread monster villages out, simmilar to towns. 
        // Dungeons - We can use this to smooth dungeons out, so that there are less or more pointy bits.
        public override void ProcessCell(IRuntimeContext context, int[] input, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth)
        {
            int ox = RequiredXBorder;
            int oy = RequiredYBorder;
            long rw = RequiredYBorder * 2 + width;

            // Does this if statement work?
            // Nevermind have to remake this entire section anyway, too many || && and its getting confusing and over extended.
            if ((input[(i + ox) + (j + oy) * rw] == Value && ExcludeValue == true) || (input[(i + ox) + (j + oy) * rw] != Value && ExcludeValue == false))
            {
                if (NeighborChancing == false || context.GetRandomRange(x, y, 1, 100, context.Modifier) < (input[(i + ox + 1) + (j + oy + 1) * rw] + input[(i + ox - 1) + (j + oy - 1) * rw] + input[(i + ox - 1) + (j + oy + 1) * rw] + input[(i + ox + 1) + (j + oy - 1) * rw]))
                {
                    if (AddPoints == false || (AddPoints == true && (NeighborChancing == false || (NeighborChancing == true && context.GetRandomRange(x, y, 1, 100, context.Modifier) > (input[(i + ox + 1) + (j + oy + 1) * rw] + input[(i + ox - 1) + (j + oy - 1) * rw] + input[(i + ox - 1) + (j + oy + 1) * rw] + input[(i + ox + 1) + (j + oy - 1) * rw])))))
                    {

                    int selected = context.GetRandomRange(x, y, 1, 4, context.Modifier);
                
                    switch (selected)
                    {
                        case 0:
                            output[i + j * width] = input[(i + ox + 1) + (j + oy + 1) * rw];
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox - 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy + 1) * rw] = input[(i + ox - 1) + (j + oy - 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy + 1) * rw] = input[(i + ox - 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox + 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy + 1) * rw] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            }
                            break;
                        case 1:
                            output[i + j * width] = input[(i + ox - 1) + (j + oy - 1) * rw];
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox + 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy - 1) * rw] = input[(i + ox + 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy - 1) * rw] = input[(i + ox - 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox + 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy - 1) * rw] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            }
                            break;
                        case 2:
                            output[i + j * width] = input[(i + ox - 1) + (j + oy + 1) * rw]; 
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox + 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy + 1) * rw] = input[(i + ox + 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy + 1) * rw] = input[(i + ox - 1) + (j + oy - 1) * rw];
                                if ((input[(i + ox + 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox - 1) + (j + oy + 1) * rw] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            }
                            break;
                        case 3:
                            output[i + j * width] = input[(i + ox + 1) + (j + oy - 1) * rw];
                            if (NeighborExpanding == true)
                            {
                                if ((input[(i + ox + 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox + 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy - 1) * rw] = input[(i + ox + 1) + (j + oy + 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy - 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy - 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy - 1) * rw] = input[(i + ox - 1) + (j + oy - 1) * rw];
                                if ((input[(i + ox - 1) + (j + oy + 1) * rw] == Value && ExcludeValue == true) || (input[(i + ox - 1) + (j + oy + 1) * rw] != Value && ExcludeValue == false))
                                    output[(i + ox + 1) + (j + oy - 1) * rw] = input[(i + ox - 1) + (j + oy + 1) * rw];
                            }
                            break;
                    }
                    }
                }
            }
            else 
                output[i + j * width] = input[(i + ox) + (j + oy) * rw];
        }
    }
}

