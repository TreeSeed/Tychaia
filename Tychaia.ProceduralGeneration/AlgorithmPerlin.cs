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
    [FlowDesignerMajorCategory(FlowMajorCategory.General)]
    [FlowDesignerCategory(FlowCategory.Initials)]
    [FlowDesignerName("Perlin Noise")]
    public class AlgorithmPerlin : Algorithm<int>
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

        [DataMember]
        [DefaultValue(true)]
        [Description("Show this layer as 2D in the editor.")]
        public bool Layer2D
        {
            get;
            set;
        }
        
        public override bool Is2DOnly
        {
            get { return this.Layer2D; }
        }
        
        public AlgorithmPerlin()
            : base()
        {
            // Set defaults.
            this.Scale = 10;
            this.Modifier = new Random().Next();
            this.MinValue = 0;
            this.MaxValue = 100;
            this.Layer2D = false;
        }

        private PerlinNoise m_PerlinNoise = null;

        public override void Initialize(IRuntimeContext context)
        {
            this.m_PerlinNoise = new PerlinNoise(context.Seed + this.Modifier);
        }

        public override void ProcessCell(IRuntimeContext context, int[] output, long x, long y, long z, int i, int j, int k, int width, int height, int depth, int ox, int oy, int oz)
        {
            if (!Layer2D)
            {
                double noise = this.m_PerlinNoise.Noise((x) / this.Scale, (y) / this.Scale, (z) / this.Scale) / 2.0 + 0.5;
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)((noise * (this.MaxValue - this.MinValue)) + this.MinValue);
            }
            else
            {
                double noise = this.m_PerlinNoise.Noise((x) / this.Scale, (y) / this.Scale, 0) / 2.0 + 0.5;
                output[(i + ox) + (j + oy) * width + (k + oz) * width * height] = (int)((noise * (this.MaxValue - this.MinValue)) + this.MinValue);
            }
        }

        public override Color GetColorForValue(StorageLayer parent, dynamic value)
        {
            return Color.FromArgb((int)(255 * (value / (float)this.MaxValue)), (int)(255 * (value / (float)this.MaxValue)), (int)(255 * (value / (float)this.MaxValue)));
        }
    }
}

