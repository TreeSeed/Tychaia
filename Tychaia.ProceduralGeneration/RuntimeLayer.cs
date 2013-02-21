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

        // Just finding offsets, then use them to determine max width, start X location, etc.
        public static void FindMaximumOffsets(
            RuntimeLayer layer, 
            out int OffsetX,
            out int OffsetY,
            out int OffsetZ)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            OffsetX = 0;
            OffsetY = 0;
            OffsetZ = 0;

            if (layer.m_Inputs.Length != 0)
            {
                int inputs = 0;
                int[] TempOffsetX = new int[layer.m_Inputs.Length];
                int[] TempOffsetY = new int[layer.m_Inputs.Length];
                int[] TempOffsetZ = new int[layer.m_Inputs.Length];

                foreach (var input in layer.m_Inputs)
                {
                    if (input == null)
                        continue;
                    
                    TempOffsetX[inputs] += layer.m_Algorithm.RequiredXBorder[inputs];
                    TempOffsetY[inputs] += layer.m_Algorithm.RequiredYBorder[inputs];
                    TempOffsetZ[inputs] += layer.m_Algorithm.RequiredZBorder[inputs];

                    FindMaximumOffsets(input, out OffsetX, out OffsetY, out OffsetZ);

                    TempOffsetX[inputs] += OffsetX;
                    TempOffsetY[inputs] += OffsetY;
                    TempOffsetZ[inputs] += OffsetZ;
                    inputs++;
                }

                for (int count = 0; count < inputs; count++)
                {
                    if (OffsetX < TempOffsetX[count])
                        OffsetX = TempOffsetX[count];
                    if (OffsetY < TempOffsetY[count])
                        OffsetY = TempOffsetY[count];
                    if (OffsetZ < TempOffsetZ[count])
                        OffsetZ = TempOffsetZ[count];
                }
            }
        }

        /// <summary>
        /// Performs the algorithm runtime call using reflection.  This is rather slow,
        /// so we should use a static compiler to prepare world configurations for
        /// release mode (in-game and MMAW).
        /// </summary>
        /// <param name="X">The absolute X value.</param>
        /// <param name="Y">The absolute Y value.</param>
        /// <param name="Z">The absolute Z value.</param>
        /// <param name="arrayWidth">The array width.</param>
        /// <param name="arrayHeight">The array height.</param>
        /// <param name="arrayDepth">The array depth.</param>
        /// <param name="MaxOffsetX">The X offset maximum from all layers.</param>
        /// <param name="MaxOffsetY">The Y offset maximum from all layers.</param>
        /// <param name="MaxOffsetZ">The Z offset maximum from all layers.</param>
        /// <param name="childOffsetX">The X offset from all previous layers.</param>
        /// <param name="childOffsetY">The Y offset from all previous layers.</param>
        /// <param name="childOffsetZ">The Z offset from all previous layers.</param>
        /// <param name="halfInputWidth">If the layer only provides half the output of width.</param>
        /// <param name="halfInputHeight">If the layer only provides half the output of height.</param>
        /// <param name="halfInputDepth">If the layer only provides half the output of depth.</param>
        private dynamic PerformAlgorithmRuntimeCall(long X, long Y, long Z,
                                                    int arrayWidth, int arrayHeight, int arrayDepth,
                                                    int MaxOffsetX, int MaxOffsetY, int MaxOffsetZ,
                                                    int childOffsetX, int childOffsetY, int childOffsetZ,
                                                    int halfInputWidth, int halfInputHeight, int halfInputDepth,
                                                    ref int computations)
        {
            // Check the generate width, height and depth. This actually doesn't work with this system anyway
            /*
            if (arrayWidth != (int)(xTo - xFrom) ||
                arrayHeight != (int)(yTo - yFrom) ||
                arrayDepth != (int)(zTo - zFrom))
                throw new InvalidOperationException("Size generation is out of sync!"); 
            */

            // Get the method for processing cells.
            dynamic algorithm = this.m_Algorithm;
            var processCell = this.m_Algorithm.GetType().GetMethod("ProcessCell");

            dynamic outputArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (int)(arrayWidth * arrayHeight * arrayDepth));

            // Depending on the argument count, invoke the method appropriately.
            switch (processCell.GetParameters().Length)
            {
                case 11: // 0 inputs
                    {
                        // context, output, x, y, z, i, j, k, width, height, depth

                        var jhalf = (halfInputHeight > 0 ? (int)(Math.Pow(2, halfInputHeight)) : 1);
                        var ihalf = (halfInputWidth > 0 ? (int)(Math.Pow(2, halfInputWidth)) : 1);
                        var khalf = (halfInputDepth > 0 ? (int)(Math.Pow(2, halfInputDepth)) : 1);

                        for (int k = 0; k < arrayDepth - (MaxOffsetZ - childOffsetZ) * 2; k++)
                            for (int i = 0; i < arrayWidth - (MaxOffsetX - childOffsetX) * 2; i++)
                                for (int j = 0; j < arrayHeight - (MaxOffsetY - childOffsetY) * 2; j++)
                                {
                                    var relativeX = i + (MaxOffsetX - childOffsetX);
                                    var relativeY = j + (MaxOffsetY - childOffsetY);
                                    var relativeZ = k + (MaxOffsetZ - childOffsetZ);
                                    var absoluteX = X + (relativeX / ihalf); 
                                    var absoluteY = Y + (relativeY / jhalf);
                                    var absoluteZ = Z + (relativeZ / khalf);

                                    algorithm.ProcessCell(this, outputArray, absoluteX, absoluteY, absoluteZ, relativeX, relativeY, relativeZ, arrayWidth, arrayHeight, arrayDepth);
                                    computations += 1;

                                }
                        break;
                    }
                case 12: // 1 input
                    {
                        // context, input, output, x, y, z, i, j, k, width, height, depth
                        if (this.m_Inputs[0] != null)
                        {
                            dynamic inputArray = this.m_Inputs[0].PerformAlgorithmRuntimeCall(
                                X,
                                Y,
                                Z,
                                arrayWidth, 
                                arrayHeight, 
                                arrayDepth,
                                MaxOffsetX,
                                MaxOffsetY,
                                MaxOffsetZ,
                                childOffsetX + this.m_Algorithm.RequiredXBorder[0],
                                childOffsetY + this.m_Algorithm.RequiredYBorder[0],
                                childOffsetZ + this.m_Algorithm.RequiredZBorder[0],
                                halfInputWidth + (this.m_Algorithm.InputWidthAtHalfSize[0] ? 1 : 0),
                                halfInputHeight + (this.m_Algorithm.InputHeightAtHalfSize[0] ? 1 : 0),
                                halfInputDepth + (this.m_Algorithm.InputDepthAtHalfSize[0] ? 1 : 0),
                                ref computations);

                            var jhalf = (halfInputHeight > 0 ? (int)(Math.Pow(2, halfInputHeight)) : 1);
                            var ihalf = (halfInputWidth > 0 ? (int)(Math.Pow(2, halfInputWidth)) : 1);
                            var khalf = (halfInputDepth > 0 ? (int)(Math.Pow(2, halfInputDepth)) : 1);

                            for (int k = 0; k < arrayDepth - (MaxOffsetZ - childOffsetZ) * 2; k++)
                                for (int i = 0; i < arrayWidth - (MaxOffsetX - childOffsetX) * 2; i++)
                                    for (int j = 0; j < arrayHeight - (MaxOffsetY - childOffsetY) * 2; j++)
                                    {
                                        var relativeX = i + (MaxOffsetX - childOffsetX);
                                        var relativeY = j + (MaxOffsetY - childOffsetY);
                                        var relativeZ = k + (MaxOffsetZ - childOffsetZ);
                                        var absoluteX = X + (relativeX / ihalf);
                                        var absoluteY = Y + (relativeY / jhalf);
                                        var absoluteZ = Z + (relativeZ / khalf);

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
                case 14: // 3 inputs
                {
                    // context, input, output, x, y, z, i, j, k, width, height, depth
                    if (this.m_Inputs[0] != null && this.m_Inputs[1] != null && this.m_Inputs[2] != null)
                    {
                        dynamic input0Array = this.m_Inputs[0].PerformAlgorithmRuntimeCall(
                            X,
                            Y,
                            Z,
                            arrayWidth, 
                            arrayHeight, 
                            arrayDepth,
                            MaxOffsetX,
                            MaxOffsetY,
                            MaxOffsetZ,
                            childOffsetX + this.m_Algorithm.RequiredXBorder[0],
                            childOffsetY + this.m_Algorithm.RequiredYBorder[0],
                            childOffsetZ + this.m_Algorithm.RequiredZBorder[0],
                            halfInputWidth + (this.m_Algorithm.InputWidthAtHalfSize[0] ? 1 : 0),
                            halfInputHeight + (this.m_Algorithm.InputHeightAtHalfSize[0] ? 1 : 0),
                            halfInputDepth + (this.m_Algorithm.InputDepthAtHalfSize[0] ? 1 : 0),
                            ref computations);

                        dynamic input1Array = this.m_Inputs[1].PerformAlgorithmRuntimeCall(
                            X,
                            Y,
                            Z,
                            arrayWidth, 
                            arrayHeight, 
                            arrayDepth,
                            MaxOffsetX,
                            MaxOffsetY,
                            MaxOffsetZ,
                            childOffsetX + this.m_Algorithm.RequiredXBorder[1],
                            childOffsetY + this.m_Algorithm.RequiredYBorder[1],
                            childOffsetZ + this.m_Algorithm.RequiredZBorder[1],
                            halfInputWidth + (this.m_Algorithm.InputWidthAtHalfSize[1] ? 1 : 0),
                            halfInputHeight + (this.m_Algorithm.InputHeightAtHalfSize[1] ? 1 : 0),
                            halfInputDepth + (this.m_Algorithm.InputDepthAtHalfSize[1] ? 1 : 0),
                            ref computations);

                        dynamic input2Array = this.m_Inputs[2].PerformAlgorithmRuntimeCall(
                            X,
                            Y,
                            Z,
                            arrayWidth, 
                            arrayHeight, 
                            arrayDepth,
                            MaxOffsetX,
                            MaxOffsetY,
                            MaxOffsetZ,
                            childOffsetX + this.m_Algorithm.RequiredXBorder[2],
                            childOffsetY + this.m_Algorithm.RequiredYBorder[2],
                            childOffsetZ + this.m_Algorithm.RequiredZBorder[2],
                            halfInputWidth + (this.m_Algorithm.InputWidthAtHalfSize[2] ? 1 : 0),
                            halfInputHeight + (this.m_Algorithm.InputHeightAtHalfSize[2] ? 1 : 0),
                            halfInputDepth + (this.m_Algorithm.InputDepthAtHalfSize[2] ? 1 : 0),
                            ref computations);

                        var jhalf = (halfInputHeight > 0 ? (int)(Math.Pow(2, halfInputHeight)) : 1);
                        var ihalf = (halfInputWidth > 0 ? (int)(Math.Pow(2, halfInputWidth)) : 1);
                        var khalf = (halfInputDepth > 0 ? (int)(Math.Pow(2, halfInputDepth)) : 1);
                        
                        for (int k = 0; k < arrayDepth - (MaxOffsetZ - childOffsetZ) * 2; k++)
                            for (int i = 0; i < arrayWidth - (MaxOffsetX - childOffsetX) * 2; i++)
                                for (int j = 0; j < arrayHeight - (MaxOffsetY - childOffsetY) * 2; j++)
                            {
                                var relativeX = i + (MaxOffsetX - childOffsetX);
                                var relativeY = j + (MaxOffsetY - childOffsetY);
                                var relativeZ = k + (MaxOffsetZ - childOffsetZ);
                                var absoluteX = X + (relativeX / ihalf);
                                var absoluteY = Y + (relativeY / jhalf);
                                var absoluteZ = Z + (relativeZ / khalf);
                                
                                algorithm.ProcessCell(
                                    this,
                                    input0Array,
                                    input1Array,
                                    input2Array,
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


            // Just replicate this into the CompiledLayer system
            int MaxOffsetX = 0;
            int MaxOffsetY = 0; 
            int MaxOffsetZ = 0;

            FindMaximumOffsets(this, out MaxOffsetX, out MaxOffsetY, out MaxOffsetZ);
            /*
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
            */

            /*
            int resultWidth = width;
            int resultHeight = height;
            int resultDepth = depth;
            int OffsetX = 0;
            int OffsetY = 0;
            int OffsetZ = 0;
*/
            // change generate width to total width generated by ranged layer
            // change offset x to total offset x generated my ranged layer
            // pass this offset x + offsetx to be offest from all childs
            // var relativeX = i + generateOffsetX - pregenOffsetX;
            // var relativeX = i + TOTALOFFSETX - this.RequiredXBorder + OffsetX

            // Need MaxOffsetX/Y/Z (derive total width/l/d from that)
            // In runtime do X + MaxOffsetX - ChildOffsetX
            // Actually added MaxOffsetX onto the I value unnessecarily
            // This causes the X value to shift with every change
            // Need to remove MaxOffsetX so I've just made X = X - MaxOffsetX

            // Actually for compiled you can just have ParentOffsetX
            // Increment that every layer
            // Reset it at the start of each for loop 
            // then add it up over each layer
            // For ( reset if () add if () add ))
            // Then it will be I + ParentOffsetX

            // Going to have to make the halfinputwidth
            // to be within the algorithms
            // can't do it from this side otherwise
            // changes the x value, rather than how much you generate
            // This shouldn't be a problem since it isn't used much anyway.
            // Can make this calculated by finding the total then subtracting its parents
            // for the compiled layer.

            // ix, etc = input of input layer
            // ix = x - offset (relativeX) = xFrom
            // iwidth = width * offsetX * 2.
            // iouterx = xTo

            //TODO: Optimization list:
            // 1) Offsets and Input at half size: Make unique to each input
            // 2) Make an offset input by some value
            //    This means that we can use the same inputs to provide different results
            //    Useful for the designer if we have a complex way to generate something that has to be done the same way multiple times
            //    Simmilar to the grouping inserting that we were going to do
            //    This can be done by a modification of the seed?

            int arrayWidth = width + MaxOffsetX * 2;
            int arrayHeight = height + MaxOffsetY * 2;
            int arrayDepth = depth + MaxOffsetZ * 2;

            dynamic resultArray = this.PerformAlgorithmRuntimeCall(
                x - MaxOffsetX,
                y - MaxOffsetY,
                z - MaxOffsetZ,
                arrayWidth,
                arrayHeight,
                arrayDepth,
                MaxOffsetX, 
                MaxOffsetY, 
                MaxOffsetZ,
                0, 0, 0,
                0, 0, 0,
                ref computations);

            // Copy the result into a properly sized array.

            dynamic correctArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (int)(width * height * depth));

            for (var k = 0; k < depth; k++)
                for (var i = 0; i < width; i++)
                    for (var j = 0; j < height; j++)
                        correctArray[i + j * width + k * width * height] =
                            resultArray[(i + MaxOffsetX) +
                            (j + MaxOffsetY) * arrayWidth +
                            (k + MaxOffsetZ) * arrayWidth * arrayHeight];

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

