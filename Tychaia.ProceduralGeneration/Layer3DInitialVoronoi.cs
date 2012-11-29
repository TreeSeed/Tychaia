using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Protogame.Noise;
using MIConvexHull;
using Protogame.Math;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Generates a layer from 3D Voronoi tessellation.
    /// </summary>
    [DataContract]
    public class Layer3DInitialVoronoi : Layer3D
    {
        [DataMember]
        [DefaultValue(2500)]
        [Description("The value which determines the chance that a point value will occur on a cell.")]
        public int PointValue
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(VoronoiResult.EdgesAndOriginals)]
        [Description("The type of data to return from the Voronoi calculations.")]
        public VoronoiResult Result
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(true)]
        [Description("Edges should be returned as cross-sectional edges (that is, the edge is returned per Z level) rather than as 3D lines through space.  This option is required for Voronoi Mixdown 3D to be able to integrate the data.")]
        public bool FormCrossSection
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(15)]
        [Description("The edge sampling range used in order to provide a consistant result across the infinite range.")]
        public int EdgeSampling
        {
            get;
            set;
        }

        [DataMember]
        [Description("The seed modifier value to apply to this Voronoi map.")]
        public int Modifier
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
        [DefaultValue(0)]
        [Description("The maximum integer value in the resulting layer.")]
        public int MaxValue
        {
            get;
            set;
        }

        public Layer3DInitialVoronoi(long seed)
            : base(seed)
        {
            // Set defaults.
            this.PointValue = 2500;
            this.EdgeSampling = 15;
            this.MinValue = 0;
            this.MaxValue = 100;
            this.Modifier = new Random().Next();
            this.Result = VoronoiResult.EdgesAndOriginals;
            this.FormCrossSection = true;
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            int[] data = new int[width * height * depth];

            // Determine output values.
            int noneOutput = 0;
            int originalOutput = 1;
            int centerOutput = (this.Result == VoronoiResult.AllValues) ? 2 : 1;
            int edgeOutput = (this.Result == VoronoiResult.AllValues) ? 3 : (this.Result == VoronoiResult.EdgesAndOriginals) ? 2 : 1;

            // Scan through the size of the array, randomly creating points.
            List<double[]> points = new List<double[]>();
            for (int i = -this.EdgeSampling; i < width + this.EdgeSampling; i++)
                for (int j = -this.EdgeSampling; j < height + this.EdgeSampling; j++)
                    for (int k = -this.EdgeSampling; k < depth + this.EdgeSampling; k++)
                    {
                        int s = this.GetRandomRange(x + i, y + j, z + k, this.PointValue, this.Modifier);
                        if (s == 0)
                        {
                            points.Add(new double[] { i, j, k });
                            if (i >= 0 && i < width &&
                                j >= 0 && j < height &&
                                k >= 0 && k < depth)
                                if (this.Result == VoronoiResult.AllValues ||
                                    this.Result == VoronoiResult.EdgesAndOriginals ||
                                    this.Result == VoronoiResult.OriginalOnly)
                                    data[i + j * width + k * width * height] = originalOutput;
                        }
                    }

            // Skip computations if we are only outputting original scatter values.
            if (this.Result == VoronoiResult.OriginalOnly)
                return data;

            try
            {
                // Compute the Voronoi diagram.
                var graph = VoronoiMesh.Create<DefaultVertex, VoronoiCell3D>(points.Select(p => new DefaultVertex { Position = p.ToArray() }));

                // Output the edges if needed.
                if (this.Result == VoronoiResult.AllValues ||
                    this.Result == VoronoiResult.EdgesAndOriginals ||
                    this.Result == VoronoiResult.EdgeOnly)
                {
                    foreach (var v in graph.Edges)
                    {
                        var a = v.Source.Circumcentre;
                        var b = v.Target.Circumcentre;

                        // Normalize vector between two points.
                        double cx = 0, cy = 0, cz = 0;
                        double sx = b[0] < a[0] ? b[0] : a[0];
                        double sy = b[0] < a[0] ? b[1] : a[1];
                        double sz = b[0] < a[0] ? b[2] : a[2];
                        double mx = b[0] > a[0] ? b[0] : a[0];
                        double my = b[0] > a[0] ? b[1] : a[1];
                        double mz = b[0] > a[0] ? b[2] : a[2];
                        double tx = b[0] > a[0] ? b[0] - a[0] : a[0] - b[0];
                        double ty = b[0] > a[0] ? b[1] - a[1] : a[1] - b[1];
                        double tz = b[0] > a[0] ? b[2] - a[2] : a[2] - b[2];
                        double length = Math.Sqrt(Math.Pow(tx, 2) + Math.Pow(ty, 2) + Math.Pow(tz, 2));
                        tx /= length;
                        ty /= length;
                        tz /= length;

                        // Iterate until we reach the target.
                        while (sx + cx < mx)// && sy + cy < my)
                        {
                            if ((int)(sx + cx) >= 0 && (int)(sx + cx) < width &&
                                (int)(sy + cy) >= 0 && (int)(sy + cy) < height &&
                                (int)(sz + cz) >= 0 && (int)(sz + cz) < depth &&
                                data[(int)(sx + cx) + (int)(sy + cy) * width + (int)(sz + cz) * width * height] == noneOutput)
                                data[(int)(sx + cx) + (int)(sy + cy) * width + (int)(sz + cz) * width * height] = edgeOutput;

                            cx += tx;
                            cy += ty;
                            cz += tz;
                        }
                    }
                }

                // Output the center points if needed.
                if (this.Result == VoronoiResult.AllValues ||
                    this.Result == VoronoiResult.CenterOnly)
                {
                    foreach (var vv in graph.Vertices)
                    {
                        foreach (var v in vv.Vertices)
                        {
                            if ((int)v.Position[0] >= 0 && (int)v.Position[0] < width &&
                                (int)v.Position[1] >= 0 && (int)v.Position[1] < height)
                                data[(int)v.Position[0] + (int)v.Position[1] * width] = centerOutput;
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // Empty array.
                return new int[width * height * depth];
            }

            // Return the result.
            return data;
        }

        private int GetPerlinRNG()
        {
            long seed = this.Seed;
            seed += this.Modifier;
            seed *= this.Modifier;
            seed += this.Modifier;
            seed *= this.Modifier;
            seed += this.Modifier;
            seed *= this.Modifier;
            return (int)seed;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            return LayerColors.Voronoi3DBrushes;
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { };
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { };
        }

        public override string ToString()
        {
            return "Initial Voronoi 3D";
        }

        public class VoronoiCell3D : TriangulationCell<DefaultVertex, VoronoiCell3D>
        {
            private double[] m_CircumcentreCache = null;

            public double[] Circumcentre
            {
                get
                {
                    if (this.m_CircumcentreCache == null)
                        this.m_CircumcentreCache = this.GetCircumcentre();
                    return this.m_CircumcentreCache;
                }
            }

            private double[] GetCircumcentre()
            {
                // Get the 4 input pairs.
                double[] a = this.Vertices[0].Position;
                double[] b = this.Vertices[1].Position;
                double[] c = this.Vertices[2].Position;
                double[] d = this.Vertices[3].Position;

                // Return result.
                CircumcentreSolver solver = new CircumcentreSolver(a, b, c, d);
                if (!solver.Valid)
                    return null;
                else
                    return solver.Centre;
            }
        }
    }
}
