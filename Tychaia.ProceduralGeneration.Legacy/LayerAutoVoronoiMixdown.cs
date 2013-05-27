using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Automatically combines a Perlin and Voronoi map in a mixdown.
    /// </summary>
    [DataContract]
    [FlowDesignerCategory(FlowCategory.General)]
    [FlowDesignerName("Auto Voronoi Mixdown")]
    public class LayerAutoVoronoiMixdown : Layer2D
    {
        [DataMember]
        public LayerInitialPerlin Perlin
        {
            get;
            set;
        }

        [DataMember]
        public LayerInitialVoronoi Voronoi
        {
            get;
            set;
        }

        [DataMember]
        public LayerVoronoiMixdown Mixdown
        {
            get;
            set;
        }

        public LayerAutoVoronoiMixdown()
            : base()
        {
            this.Perlin = new LayerInitialPerlin();
            this.Voronoi = new LayerInitialVoronoi();
            this.Mixdown = new LayerVoronoiMixdown(this.Voronoi, this.Perlin);
            this.Perlin.Seed = this.Seed;
            this.Voronoi.Seed = this.Seed;
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            // If we are deserialized, our Perlin / Voronoi might not be created
            // so we need to recreate them.
            if (this.Perlin == null)
            {
                this.Perlin = new LayerInitialPerlin();
                this.Perlin.Seed = this.Seed;
            }
            if (this.Voronoi == null)
            {
                this.Voronoi = new LayerInitialVoronoi();
                this.Voronoi.Seed = this.Seed;
            }
            if (this.Mixdown == null)
                this.Mixdown = new LayerVoronoiMixdown(this.Voronoi, this.Perlin);

            return this.Mixdown.GenerateData(x, y, width, height);
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            // If we are deserialized, our Perlin / Voronoi might not be created
            // so we need to recreate them.
            if (this.Perlin == null)
            {
                this.Perlin = new LayerInitialPerlin();
                this.Perlin.Seed = this.Seed;
            }
            if (this.Voronoi == null)
            {
                this.Voronoi = new LayerInitialVoronoi();
                this.Voronoi.Seed = this.Seed;
            }
            if (this.Mixdown == null)
                this.Mixdown = new LayerVoronoiMixdown(this.Voronoi, this.Perlin);

            return this.Mixdown.GetLayerColors();
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { };
        }

        public override string ToString()
        {
            return "Auto Voronoi Mixdown";
        }
    }
}
