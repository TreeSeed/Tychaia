using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Smooths the input layer.
    /// </summary>
    [DataContract]
    public class LayerSmooth : Layer2D
    {
        [DataMember]
        [DefaultValue(SmoothType.Linear)]
        [Description("The smoothing algorithm to use.")]
        public SmoothType Mode {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(1)]
        [Description("The number of smooth iterations to perform.")]
        public int Iterations {
            get;
            set;
        }

        public LayerSmooth(Layer parent)
            : base(parent)
        {
            this.Mode = SmoothType.Linear;
            this.Iterations = 1;
        }

        private int[] GenerateDataIterate(int iter, long x, long y, long width, long height)
        {
            int ox = 2; // Offsets
            int oy = 2;
            long rw = width + ox * 2;
            long rh = height + oy * 2;

            // For smoothing to work, we need to know the cells that are actually
            // beyond the edge of the center.
            int[] parent = null;
            if (iter == this.Iterations)
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                parent = new int[rw * rh];
            else
                parent = this.Parents[0].GenerateData(x - ox, y - oy, rw, rh);
            else
                parent = this.GenerateDataIterate(iter + 1, x - ox, y - oy, rw, rh);
            int[] data = new int[width * height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    switch (this.Mode)
                    {
                        case SmoothType.None:
                            data[i + j * width] = this.SmoothNone(parent, i + ox, j + oy, rw);
                            break;
                        case SmoothType.Linear:
                            data[i + j * width] = this.SmoothLinear(parent, i + ox, j + oy, rw);
                            break;
                        case SmoothType.Parabolic:
                            data[i + j * width] = this.SmoothParabolic(parent, i + ox, j + oy, rw);
                            break;
                        case SmoothType.Cubic:
                            data[i + j * width] = this.SmoothCubic(parent, i + ox, j + oy, rw);
                            break;
                        case SmoothType.Random:
                            data[i + j * width] = this.SmoothRandom(parent, i + ox, j + oy, rw);
                            break;
                    }
      
            return data;
        }

        private int SmoothNone(int[] source, int x, int y, long rw)
        {
            return source[x + y * rw];
        }

        private static int[,] m_LinearApplier = new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1 },
        };
        private static int[,] m_ParabolicApplier = new int[,]
        {
            { 1, 4, 9, 4, 1 },
            { 4, 9, 16, 9, 4 },
            { 9, 16, 25, 16, 9 },
            { 4, 9, 16, 9, 4 },
            { 1, 4, 9, 4, 1 }
        };
        private static int[,] m_CubicApplier = new int[,]
        {
            { 1, 8, 27, 8, 1 },
            { 8, 27, 64, 27, 8 },
            { 27, 64, 125, 64, 27 },
            { 8, 27, 64, 27, 8 },
            { 1, 8, 27, 8, 1 },
        };

        private int SmoothLinear(int[] parent, int x, int y, long rw)
        {
            return this.SmoothBase(parent, m_LinearApplier, x, y, rw);
        }

        private int SmoothParabolic(int[] parent, int x, int y, long rw)
        {
            return this.SmoothBase(parent, m_ParabolicApplier, x, y, rw);
        }

        private int SmoothCubic(int[] parent, int x, int y, long rw)
        {
            return this.SmoothBase(parent, m_CubicApplier, x, y, rw);
        }

        private int SmoothRandom(int[] parent, int x, int y, long rw)
        {
            int[,] applier = new int[5, 5];
            for (int i = -2; i <= 2; i++)
                for (int j = -2; j <= 2; j++)
                    applier[i + 2, j + 2] = this.GetRandomRange(x + i, y + j, 0, 250);
            return this.SmoothBase(parent, applier, x, y, rw);
        }
     
        private int[,] m_BaseSample = new int[5, 5];
        private int[,] m_BaseOutput = new int[5, 5];
     
        private int SmoothBase(int[] parent, int[,] applier, int x, int y, long rw)
        {
            int result = 0;
            int total = 0;
            if (m_BaseSample == null)
                m_BaseSample = new int[5, 5];
            if (m_BaseOutput == null)
                m_BaseOutput = new int[5, 5];
         
            for (int i = -2; i <= 2; i++)
                for (int j = -2; j <= 2; j++)
                {
                    try
                    {
                        m_BaseSample[i + 2, j + 2] = parent[(x + i) + (y + j) * rw];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new Exception("problem assigning base sample when " + (i + 2).ToString() +
                                     ", " + (j + 2).ToString() + " accessed.  length of base array is " + m_BaseSample.Length, e);
                    }
                }
            foreach (int v in applier)
                total += v;
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    try
                    {
                        m_BaseOutput[i, j] = m_BaseSample[i, j] * applier[i, j];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new Exception("problem calculating result when " + (i).ToString() +
                                     ", " + (j).ToString() + " accessed.  length of applier is " + applier.Length, e);
                    }
            foreach (int v in m_BaseOutput)
                result += v;

            return (int)((double)result / (double)total);
        }

        public override Dictionary<int, LayerColor> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return this.Parents[0].GetLayerColors();
        }

        protected override int[] GenerateDataImpl(long x, long y, long width, long height)
        {
            if (this.Iterations > 0)
                return this.GenerateDataIterate(1, x, y, width, height);
            else if (this.Parents.Length < 1 || this.Parents[0] == null)
                return new int[width * height];
            else
                return this.Parents[0].GenerateData(x, y, width, height);
        }

        public override string ToString()
        {
            return "Smooth";
        }

        /// <summary>
        /// An enumeration defining the type of smooth to perform.
        /// </summary>
        public enum SmoothType
        {
            None,
            Linear,
            Parabolic,
            Cubic,
            Random
        }
    }
}
