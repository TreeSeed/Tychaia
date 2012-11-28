using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Performs mathematical operations on noise maps.
    /// </summary>
    [DataContract]
    public class Layer3DPerformMath : Layer3D
    {
        [DataMember]
        [DefaultValue(MathOp.Add)]
        [Description("The mathematical operation to perform.")]
        public MathOp MathOp
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the first noise map.")]
        public int MinInputFirst
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the first noise map.")]
        public int MaxInputFirst
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the second noise map.")]
        public int MinInputSecond
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the second noise map.")]
        public int MaxInputSecond
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the output map.")]
        public int MinOutput
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the output map.")]
        public int MaxOutput
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The constant numeric value to use for SetNumber operation.")]
        public int Constant
        {
            get;
            set;
        }

        public Layer3DPerformMath(Layer first, Layer second)
            : base(new Layer[] { first, second })
        {
            this.MathOp = MathOp.Add;
            this.MinInputFirst = 0;
            this.MaxInputFirst = 100;
            this.MinInputSecond = 0;
            this.MaxInputSecond = 100;
            this.MinOutput = 0;
            this.MaxOutput = 100;
            this.Constant = 0;
        }

        protected override int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            int[] first = null;
            int[] second = null;
            if (!(this.MathOp == ProceduralGeneration.MathOp.SetNumber))
                if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                    return new int[width * height * depth];

            if (!(this.MathOp == ProceduralGeneration.MathOp.SetNumber))
            {
                first = this.Parents[0].GenerateData(x, y, z, width, height, depth);
                second = this.Parents[1].GenerateData(x, y, z, width, height, depth);
            }
            int[] data = new int[width * height * depth];

            // Perform the mathematical operation.
            for (long i = 0; i < width; i++)
                for (long j = 0; j < height; j++)
                    for (long k = 0; k < depth; k++)
                        try
                        {
                            // Convert values to doubles.
                            double a = 0, b = 0, val;
                            if (!(this.MathOp == ProceduralGeneration.MathOp.SetNumber))
                            {
                                a = (first[i + j * width + k * width * height] - this.MinInputFirst) / (double)(this.MaxInputFirst - this.MinInputFirst);
                                b = (second[i + j * width + k * width * height] - this.MinInputSecond) / (double)(this.MaxInputSecond - this.MinInputSecond);
                            }

                            // Do operation.
                            switch (this.MathOp)
                            {
                                case ProceduralGeneration.MathOp.Add:
                                    val = a + b;
                                    break;
                                case ProceduralGeneration.MathOp.Subtract:
                                    val = a - b;
                                    break;
                                case ProceduralGeneration.MathOp.Multiply:
                                    val = a * b;
                                    break;
                                case ProceduralGeneration.MathOp.Divide:
                                    val = a / b;
                                    break;
                                case ProceduralGeneration.MathOp.SetNumber:
                                    val = this.Constant;
                                    break;
                                default:
                                    val = 0;
                                    break;
                            }
                        
                            // Store result.
                            data[i + j * width + k * width * height] = (int)(val * (this.MaxOutput - this.MinOutput) + this.MinOutput);
                        }
                        catch (Exception)
                        {
                            // In case of overflow, underflow or divide by zero.
                            data[i + j * width + k * width * height] = 0;
                        }

            return data;
        }

        public override Dictionary<int, System.Drawing.Brush> GetLayerColors()
        {
            if (this.Parents.Length < 1 || this.Parents[0] == null)
                return null;
            else
                return this.Parents[0].GetLayerColors();
        }

        public override bool[] GetParents3DRequired()
        {
            return new bool[] { true, true };
        }

        public override string[] GetParentsRequired()
        {
            return new string[] { "First", "Second" };
        }

        public override string ToString()
        {
            return "Perform Math 3D";
        }
    }
}
