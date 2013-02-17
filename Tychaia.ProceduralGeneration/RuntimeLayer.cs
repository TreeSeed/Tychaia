//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Tychaia.ProceduralGeneration.Compiler;
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration
{
    public class RuntimeLayer : IRuntimeContext, IGenerator
    {
        /// <summary>
        /// The current algorithm for this layer.
        /// </summary>
        [DataMember]
        private IAlgorithm
            m_Algorithm;

        /// <summary>
        /// The input layers.
        /// </summary>
        [DataMember]
        private RuntimeLayer[]
            m_Inputs;

        /// <summary>
        /// The current algorithm that this runtime layer is using.
        /// </summary>
        public IAlgorithm Algorithm
        {
            get { return this.m_Algorithm; }
        }

        /// <summary>
        /// Creates a new runtime layer that holds the specified algorithm.
        /// </summary>
        public RuntimeLayer(IAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            this.m_Algorithm = algorithm;
            var inputs = new List<RuntimeLayer>();
            for (var i = 0; i < algorithm.InputTypes.Length; i++)
                inputs.Add(null);
            this.m_Inputs = inputs.ToArray();

            this.Seed = 100;
            this.Modifier = 0;
        }

        /// <summary>
        /// Determines whether or not the specified input algorithm can be used as an
        /// input for the current algorithm in the specified index slot.
        /// </summary>
        public bool CanBeInput(int index, RuntimeLayer input)
        {
            if (input == null)
                return true;
            if (index < 0 || index >= this.m_Algorithm.InputTypes.Length)
                return false;
            return (input.m_Algorithm.OutputType == this.m_Algorithm.InputTypes[index]);
        }

        /// <summary>
        /// Sets the specified algorithm as the input at the specified index.
        /// </summary>
        public void SetInput(int index, RuntimeLayer input)
        {
            if (!this.CanBeInput(index, input))
                throw new InvalidOperationException("Specified algorithm can not be set as input at this index.");
            this.m_Inputs[index] = input;
        }

        /// <summary>
        /// Returns the current list of inputs for this runtime layer.
        /// </summary>
        public RuntimeLayer[] GetInputs()
        {
            if (this.m_Inputs == null)
            {
                var inputs = new List<RuntimeLayer>();
                for (var i = 0; i < this.m_Algorithm.InputTypes.Length; i++)
                    inputs.Add(null);
                this.m_Inputs = inputs.ToArray();
            }
            return this.m_Inputs;
        }

        /// <summary>
        /// The modifier used by algorithms as an additional input to the
        /// random function calls.
        /// </summary>
        [DataMember]
        [Description("The seed modifier value to apply.")]
        public long Modifier
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the algorithm runtime call using reflection.  This is rather slow,
        /// so we should use a static compiler to prepare world configurations for
        /// release mode (in-game and MMAW).
        /// </summary>
        /// <param name="xFrom">The minimum X value.</param>
        /// <param name="yFrom">The minimum Y value.</param>
        /// <param name="zFrom">The minimum Z value.</param>
        /// <param name="xTo">The maximum X value.</param>
        /// <param name="yTo">The maximum Y value.</param>
        /// <param name="zTo">The maximum Z value.</param>
        /// <param name="generateWidth">The width to generate inside the array.</param>
        /// <param name="generateHeight">The height to generate inside the array.</param>
        /// <param name="generateDepth">The depth to generate inside the array.</param>
        /// <param name="generateOffsetX">The X offset to generate inside the array.</param>
        /// <param name="generateOffsetY">The Y offset to generate inside the array.</param>
        /// <param name="generateOffsetZ">The Z offset to generate inside the array.</param>
        /// <param name="arrayWidth">The array width.</param>
        /// <param name="arrayHeight">The array height.</param>
        /// <param name="arrayDepth">The array depth.</param>
        private dynamic PerformAlgorithmRuntimeCall(long xFrom, long yFrom, long zFrom,
                                                    long xTo, long yTo, long zTo,
                                                    int generateWidth, int generateHeight, int generateDepth,
                                                    int generateOffsetX, int generateOffsetY, int generateOffsetZ,
                                                    int arrayWidth, int arrayHeight, int arrayDepth,
                                                    ref int computations)
        {
            // Check the generate width, height and depth.
            if (generateWidth != (int)(xTo - xFrom) ||
                generateHeight != (int)(yTo - yFrom) ||
                generateDepth != (int)(zTo - zFrom))
                throw new InvalidOperationException("Size generation is out of sync!");

            // Get the method for processing cells.
            dynamic algorithm = this.m_Algorithm;
            var processCell = this.m_Algorithm.GetType().GetMethod("ProcessCell");

            // Create a new array of the specified array width / height / depth.
            dynamic outputArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (int)(arrayWidth * arrayHeight * arrayDepth));

            // Depending on the argument count, invoke the method appropriately.
            switch (processCell.GetParameters().Length)
            {
                case 11:
                    {
                        // context, output, x, y, z, i, j, k, width, height, depth
                        for (int k = 0; k < zTo - zFrom; k++)
                            for (int i = 0; i < xTo - xFrom; i++)
                                for (int j = 0; j < yTo - yFrom; j++)
                                {
                                    algorithm.ProcessCell(this, outputArray, i + xFrom, j + yFrom, k + zFrom, i, j, k, arrayWidth, arrayHeight, arrayDepth);
                                    computations += 1;
                                }
                        break;
                    }
                case 12:
                    {
                        // context, input, output, x, y, z, i, j, k, width, height, depth
                        if (this.m_Inputs[0] != null)
                        {
                            dynamic inputArray = this.m_Inputs[0].PerformAlgorithmRuntimeCall(
                                xFrom - this.m_Algorithm.RequiredXBorder,
                                yFrom - this.m_Algorithm.RequiredYBorder,
                                zFrom - this.m_Algorithm.RequiredZBorder,
                            generateWidth / (this.m_Algorithm.InputWidthAtHalfSize ? 2 : 1) + this.m_Algorithm.RequiredXBorder,
                            generateHeight / (this.m_Algorithm.InputHeightAtHalfSize ? 2 : 1) + this.m_Algorithm.RequiredYBorder,
                            generateDepth / (this.m_Algorithm.InputDepthAtHalfSize ? 2 : 1) + this.m_Algorithm.RequiredZBorder,
                                generateWidth / (this.m_Algorithm.InputWidthAtHalfSize ? 2 : 1) + this.m_Algorithm.RequiredXBorder * 2, 
                                generateHeight / (this.m_Algorithm.InputHeightAtHalfSize ? 2 : 1) + this.m_Algorithm.RequiredYBorder * 2, 
                                generateDepth / (this.m_Algorithm.InputDepthAtHalfSize ? 2 : 1) + this.m_Algorithm.RequiredZBorder * 2,
                                generateOffsetX - this.m_Algorithm.RequiredXBorder,
                                generateOffsetY - this.m_Algorithm.RequiredYBorder,
                                generateOffsetZ - this.m_Algorithm.RequiredZBorder,
                                arrayWidth,
                                arrayHeight,
                                arrayDepth,
                                ref computations);
                            for (var k = 0; k < generateDepth; k++)
                                for (var i = 0; i < generateWidth; i++)
                                    for (var j = 0; j < generateHeight; j++)
                                    {
                                        var absoluteX = xFrom + i;
                                        var absoluteY = yFrom + j;
                                        var absoluteZ = zFrom + k;
                                        var relativeX = i;
                                        var relativeY = j;
                                        var relativeZ = k;

                                        algorithm.ProcessCell(
                                            this,
                                            inputArray,
                                            outputArray,
                                            absoluteX,
                                            absoluteY,
                                            absoluteZ,
                                            relativeX,
                                            relativeY,
                                            relativeZ,
                                            arrayWidth,
                                            arrayHeight,
                                            arrayDepth);
                                        computations += 1;
                                    }
                        }
                        break;
                    }
                default:
                    // FIXME!
                    throw new NotImplementedException();
            }
            return outputArray;
        }

        /// <summary>
        /// Generates data using the current algorithm.
        /// </summary>
        public dynamic GenerateData(long x, long y, long z, int width, int height, int depth, out int computations)
        {
            // Initialize the computation count.
            computations = 0;

            // Work out the maximum bounds of the array.
            var ranged = new RangedLayer(this);
            Expression ix, iy, iz, iwidth, iheight, idepth, iouterx, ioutery, iouterz;
            RangedLayer.FindMaximumBounds(ranged, out ix, out iy, out iz, out iwidth, out iheight, out idepth, out iouterx, out ioutery, out iouterz);

            // Perform the algorithm calculations.
            int resultOffsetX = (int)RangedLayer.EvaluateExpression(ix, new Dictionary<string, object> { { "x", x } }).Value;
            int resultOffsetY = (int)RangedLayer.EvaluateExpression(iy, new Dictionary<string, object> { { "y", y } }).Value;
            int resultOffsetZ = (int)RangedLayer.EvaluateExpression(iz, new Dictionary<string, object> { { "z", z } }).Value;
            int resultWidth = (int)RangedLayer.EvaluateExpression(iwidth, new Dictionary<string, object> { { "x", x }, { "width", width } }).Value;
            int resultHeight = (int)RangedLayer.EvaluateExpression(iheight, new Dictionary<string, object> { { "y", y }, { "height", height } }).Value;
            int resultDepth = (int)RangedLayer.EvaluateExpression(idepth, new Dictionary<string, object> { { "z", z }, { "depth", depth } }).Value;
            resultWidth -= resultOffsetX;  // Sometimes the width doesn't encapsulate the whole region we need
            resultHeight -= resultOffsetY; // since the upper layers require the lower layers to be filled in
            resultDepth -= resultOffsetZ;  // the offset areas.
            dynamic resultArray = this.PerformAlgorithmRuntimeCall(
                x,
                y,
                z,
                x + width,
                y + height,
                z + depth,
                width,
                height,
                depth,
                resultOffsetX,
                resultOffsetY,
                resultOffsetZ,
                resultWidth,
                resultHeight,
                resultDepth,
                ref computations);

            // Copy the result into a properly sized array.
            dynamic correctArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (int)(width * height * depth));
            for (var k = 0; k < depth; k++)
                for (var i = 0; i < width; i++)
                    for (var j = 0; j < height; j++)
                        correctArray[i + j * width + k * width * height] =
                            resultArray[(i + -resultOffsetX) +
                            (j + -resultOffsetY) * resultWidth +
                            (k + -resultOffsetZ) * resultWidth * resultDepth];

            // Return the result.
            return correctArray;
        }
        
        #region Randomness
        
        private long m_Seed;
        
        /// <summary>
        /// The world seed.
        /// </summary>
        public long Seed
        {
            get
            {
                return this.m_Seed;
            }
            set
            {
                this.m_Seed = value;
            }
        }
        
        /// <summary>
        /// Returns a random positive integer between the specified 0 and
        /// the exclusive end value.
        /// </summary>
        public int GetRandomRange(long x, long y, long z, int end, long modifier = 0)
        {
            return AlgorithmUtility.GetRandomRange(this.Seed, x, y, z, end, modifier);
        }
        
        /// <summary>
        /// Returns a random positive integer between the specified inclusive start
        /// value and the exclusive end value.
        /// </summary>
        public int GetRandomRange(long x, long y, long z, int start, int end, long modifier)
        {
            return AlgorithmUtility.GetRandomRange(this.Seed, x, y, z, start, end, modifier);
        }
        
        /// <summary>
        /// Returns a random integer over the range of valid integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        public int GetRandomInt(long x, long y, long z, long modifier = 0)
        {
            return AlgorithmUtility.GetRandomInt(this.Seed, x, y, z, modifier);
        }
        
        /// <summary>
        /// Returns a random long integer over the range of valid long integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        public long GetRandomLong(long x, long y, long z, long modifier = 0)
        {
            return AlgorithmUtility.GetRandomLong(this.Seed, x, y, z, modifier);
        }
        
        /// <summary>
        /// Returns a random double between the range of 0.0 and 1.0 based on
        /// the provided X and Y position, and the specified modifier.
        /// </summary>
        public double GetRandomDouble(long x, long y, long z, long modifier = 0)
        {
            return AlgorithmUtility.GetRandomDouble(this.Seed, x, y, z, modifier);
        }
        
        #endregion

        #region Other
        
        /// <summary>
        /// Smoothes the specified data according to smoothing logic.  Apparently
        /// inlining this functionality causes the algorithms to run slower, so we
        /// leave this function on it's own.
        /// </summary>
        public int Smooth(bool isFuzzy, long x, long y, int northValue, int southValue, int westValue, int eastValue, int southEastValue, int currentValue, long i, long j, long ox, long oy, long rw, int[] parent)
        {
            // Parent-based Smoothing
            int selected = 0;
            
            if (x % 2 == 0)
            {
                if (y % 2 == 0)
                {
                    return currentValue;
                }
                else
                {
                    selected = this.GetRandomRange(x, y, 0, 2);
                    switch (selected)
                    {
                        case 0:
                            return currentValue;
                        case 1:
                            return southValue;
                    }
                }
            }
            else
            {
                if (y % 2 == 0)
                {
                    selected = this.GetRandomRange(x, y, 0, 2);
                    switch (selected)
                    {
                        case 0:
                            return currentValue;
                        case 1:
                            return eastValue;
                    }
                }
                else
                {
                    if (!isFuzzy)
                    {
                        selected = this.GetRandomRange(x, y, 0, 3);
                        switch (selected)
                        {
                            case 0:
                                return currentValue;
                            case 1:
                                return southValue;
                            case 2:
                                return eastValue;
                        }
                    }
                    else
                    {
                        selected = this.GetRandomRange(x, y, 0, 4);
                        switch (selected)
                        {
                            case 0:
                                return currentValue;
                            case 1:
                                return southValue;
                            case 2:
                                return eastValue;
                            case 3:
                                return southEastValue;
                        }
                    }
                }
            }
            
            // Select one of the four options if we couldn't otherwise
            // determine a value.
            selected = this.GetRandomRange(x, y, 0, 4);
            
            switch (selected)
            {
                case 0:
                    return northValue;
                case 1:
                    return southValue;
                case 2:
                    return eastValue;
                case 3:
                    return westValue;
            }
            
            throw new InvalidOperationException();
        }
        
        #endregion

    }
}

