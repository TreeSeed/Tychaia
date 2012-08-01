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
    public class LayerAutoVoronoiMixdown : Layer
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

        public LayerAutoVoronoiMixdown(long seed)
            : base(seed)
        {
            this.Perlin = new LayerInitialPerlin(seed);
            this.Voronoi = new LayerInitialVoronoi(seed);
            this.Mixdown = new LayerVoronoiMixdown(this.Voronoi, this.Perlin);
        }

        public override int[] GenerateData(int x, int y, int width, int height)
        {
            // If we are deserialized, our Perlin / Voronoi might not be created
            // so we need to recreate them.
            if (this.Perlin == null)
                this.Perlin = new LayerInitialPerlin(this.Seed);
            if (this.Voronoi == null)
                this.Voronoi = new LayerInitialVoronoi(this.Seed);
            if (this.Mixdown == null)
                this.Mixdown = new LayerVoronoiMixdown(this.Voronoi, this.Perlin);

            return this.Mixdown.GenerateData(x, y, width, height);
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            // If we are deserialized, our Perlin / Voronoi might not be created
            // so we need to recreate them.
            if (this.Perlin == null)
                this.Perlin = new LayerInitialPerlin(this.Seed);
            if (this.Voronoi == null)
                this.Voronoi = new LayerInitialVoronoi(this.Seed);
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
