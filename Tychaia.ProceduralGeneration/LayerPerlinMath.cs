using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    /// <summary>
    /// Performs mathematical operations on perlin noise maps.
    /// </summary>
    [DataContract]
    public class LayerPerlinMath : Layer2D
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
        [Description("The minimum integer value in the first perlin noise map.")]
        public int MinPerlinFirst
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the first perlin noise map.")]
        public int MaxPerlinFirst
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the second perlin noise map.")]
        public int MinPerlinSecond
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the second perlin noise map.")]
        public int MaxPerlinSecond
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(0)]
        [Description("The minimum integer value in the perlin output map.")]
        public int MinPerlinOutput
        {
            get;
            set;
        }

        [DataMember]
        [DefaultValue(100)]
        [Description("The maximum integer value in the perlin output map.")]
        public int MaxPerlinOutput
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

        public LayerPerlinMath(Layer first, Layer second)
            : base(new Layer[] { first, second })
        {
            this.MathOp = MathOp.Add;
            this.MinPerlinFirst = 0;
            this.MaxPerlinFirst = 100;
            this.MinPerlinSecond = 0;
            this.MaxPerlinSecond = 100;
            this.MinPerlinOutput = 0;
            this.MaxPerlinOutput = 100;
            this.Constant = 0;
        }

        protected override int[] GenerateDataImpl(int x, int y, int width, int height)
        {
            int[] first = null;
            int[] second = null;
            if (!(this.MathOp == ProceduralGeneration.MathOp.SetNumber))
                if (this.Parents.Length < 2 || this.Parents[0] == null || this.Parents[1] == null)
                    return new int[width * height];

            if (!(this.MathOp == ProceduralGeneration.MathOp.SetNumber))
            {
                first = this.Parents[0].GenerateData(x, y, width, height);
                second = this.Parents[1].GenerateData(x, y, width, height);
            }
            int[] data = new int[width * height];

            // Perform the mathematical operation.
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    try
                    {
                        // Convert values to doubles.
                        double a = 0, b = 0, val;
                        if (!(this.MathOp == ProceduralGeneration.MathOp.SetNumber))
                        {
                            a = (first[i + j * width] - this.MinPerlinFirst) / (double)(this.MaxPerlinFirst - this.MinPerlinFirst);
                            b = (second[i + j * width] - this.MinPerlinSecond) / (double)(this.MaxPerlinSecond - this.MinPerlinSecond);
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
                        data[i + j * width] = (int)(val * (this.MaxPerlinOutput - this.MinPerlinOutput) + this.MinPerlinOutput);
                    }
                    catch (Exception)
                    {
                        // In case of overflow, underflow or divide by zero.
                        data[i + j * width] = 0;
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

        public override string[] GetParentsRequired()
        {
            return new string[] { "First", "Second" };
        }

        public override string ToString()
        {
            return "Perlin Math";
        }
    }

    public enum MathOp
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        SetNumber
    }
}
