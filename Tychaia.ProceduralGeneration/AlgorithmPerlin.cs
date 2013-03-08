//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;
using Protogame.Noise;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    [FlowDesignerCategory(FlowCategory.BaseLayers)]
    [FlowDesignerName("Rough Perlin")]
    public class AlgorithmPerlinRough : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(1)]
        [Description("The scale of the perlin noise map.")]
        public int Scale
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }
        
        public AlgorithmPerlinRough()
        {
            this.Scale = 1;
        }

        public override void Initialize()
        {
        }

        // Will be able to use this algorithm for:
        // Land - This is the equivelent of InitialLand
        // Towns - This is the equivelent of InitialTowns
        // Landmarks - We can spread landmarks over the world, which we can then use values to determine the size/value of the landmarks.
        // Monsters - By utilising multiple value scaling we can either distribute individual monsters or monster groups or even monster villages.
        // Tresure chests - Spreading tresure chests in dungeons (can be used as an estimated location then moved slightly too).
        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            double result = 0;
            double b = 16;

            if (Scale < 1)
                Scale = 1;

            for (int a = 1; a <= b; a *= 2)
                result += (context.GetRandomDouble(x / (a * Scale), y / (a * Scale), z / (a * Scale), context.Modifier) / (double)(b / a));

            result /= 1.96875;

            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)((result) * 100);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb((int)(255 * (value / 100f)), (int)(255 * (value / 100f)), (int)(255 * (value / 100f)));
        }
    }

    [DataContract]
    [FlowDesignerCategory(FlowCategory.BaseLayers)]
    [FlowDesignerName("Smooth Perlin")]
    public class AlgorithmPerlinSmooth : Algorithm<int>
    {
        [DataMember]
        [DefaultValue(10)]
        [Description("The scale of the perlin noise map.")]
        public double Scale
        {
            get;
            set;
        }
        
        [DataMember]
        [Description("The seed modifier value to apply to this perlin map.")]
        public long Modifier
        {
            get;
            set;
        }
        
        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the resulting layer.")]
        public int MinValue
        {
            get;
            set;
        }
        
        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the resulting layer.")]
        public int MaxValue
        {
            get;
            set;
        }

        public override bool Is2DOnly
        {
            get { return false; }
        }
        
        public AlgorithmPerlinSmooth()
            : base()
        {
            // Set defaults.
            this.Scale = 10;
            this.Modifier = new Random().Next();
            this.MinValue = 0;
            this.MaxValue = 100;
        }

        private PerlinNoise perlin = new PerlinNoise(0);

        public override void Initialize()
        {
            PerlinNoise perlin = new PerlinNoise(this.Modifier);
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            double noise = perlin.Noise((x) / this.Scale, (y) / this.Scale, (z) / this.Scale) / 2.0 + 0.5;
            output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)((noise * (this.MaxValue - this.MinValue)) + this.MinValue);
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb((int)(255 * (value / 100f)), (int)(255 * (value / 100f)), (int)(255 * (value / 100f)));
        }
    }
}

