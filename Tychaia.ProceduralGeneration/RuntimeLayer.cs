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
using Protogame.Noise;

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
        /// Occurs when data has been generated for an algorithm.
        /// </summary>
        public event DataGeneratedEventHandler DataGenerated;

        /// <summary>
        /// Arbitrary userdata to associate with this object.
        /// </summary>
        public object Userdata;

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

            this.Seed = 0;
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
        /// <remarks>
        /// This function also automatically updates the seed value for the
        /// new input layer as well.
        /// </remarks>
        public void SetInput(int index, RuntimeLayer input)
        {
            if (!this.CanBeInput(index, input))
                throw new InvalidOperationException("Specified algorithm can not be set as input at this index.");
            this.m_Inputs[index] = input;
            if (this.m_Inputs[index] != null)
                this.m_Inputs[index].SetSeed(this.Seed);
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
            out int offsetX,
            out int offsetY,
            out int offsetZ,
            out int halfX, 
            out int halfY, 
            out int halfZ)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            offsetX = 0;
            offsetY = 0;
            offsetZ = 0;
            halfX = 0;
            halfY = 0;
            halfZ = 0;

            if (layer.m_Inputs.Length != 0)
            {
                int inputs = 0;
                var TempOffsetX = new int[layer.m_Inputs.Length];
                var TempOffsetY = new int[layer.m_Inputs.Length];
                var TempOffsetZ = new int[layer.m_Inputs.Length];
                var TempHalfX = new int[layer.m_Inputs.Length];
                var TempHalfY = new int[layer.m_Inputs.Length];
                var TempHalfZ = new int[layer.m_Inputs.Length];

                foreach (var input in layer.m_Inputs)
                {
                    if (input == null)
                        continue;

                    // can't just divide offsets after half by half
                    // 

//                    TempOffsetX[inputs] += (layer.m_Algorithm.InputWidthAtHalfSize[inputs] ? Math.Abs(layer.m_Algorithm.RequiredXBorder[inputs]) * 2 : Math.Abs(layer.m_Algorithm.RequiredXBorder[inputs]));
//                    TempOffsetY[inputs] += (layer.m_Algorithm.InputHeightAtHalfSize[inputs] ? Math.Abs(layer.m_Algorithm.RequiredYBorder[inputs]) * 2 : Math.Abs(layer.m_Algorithm.RequiredYBorder[inputs]));
//                    TempOffsetZ[inputs] += (layer.m_Algorithm.InputDepthAtHalfSize[inputs] ? Math.Abs(layer.m_Algorithm.RequiredZBorder[inputs]) * 2 : Math.Abs(layer.m_Algorithm.RequiredZBorder[inputs]));
                    
                    TempHalfX[inputs] += (layer.m_Algorithm.InputWidthAtHalfSize[inputs] ? 1 : 0);
                    TempHalfY[inputs] += (layer.m_Algorithm.InputHeightAtHalfSize[inputs] ? 1 : 0);
                    TempHalfZ[inputs] += (layer.m_Algorithm.InputDepthAtHalfSize[inputs] ? 1 : 0);
                    TempOffsetX[inputs] += Math.Abs(layer.m_Algorithm.RequiredXBorder[inputs]);
                    TempOffsetY[inputs] += Math.Abs(layer.m_Algorithm.RequiredYBorder[inputs]);
                    TempOffsetZ[inputs] += Math.Abs(layer.m_Algorithm.RequiredZBorder[inputs]);

                    FindMaximumOffsets(input, out offsetX, out offsetY, out offsetZ, out halfX, out halfY, out halfZ);

                    TempOffsetX[inputs] += offsetX;
                    TempOffsetY[inputs] += offsetY;
                    TempOffsetZ[inputs] += offsetZ;
                    TempHalfX[inputs] += halfX;
                    TempHalfY[inputs] += halfY;
                    TempHalfZ[inputs] += halfZ;
                    inputs++;
                }

                for (int count = 0; count < inputs; count++)
                {
                    if (offsetX < TempOffsetX[count])
                        offsetX = TempOffsetX[count];
                    if (offsetY < TempOffsetY[count])
                        offsetY = TempOffsetY[count];
                    if (offsetZ < TempOffsetZ[count])
                        offsetZ = TempOffsetZ[count];
                    if (halfX < TempHalfX[count])
                        halfX = TempHalfX[count];
                    if (halfY < TempHalfY[count])
                        halfY = TempHalfY[count];
                    if (halfZ < TempHalfZ[count])
                        halfZ = TempHalfZ[count]; 
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
        private dynamic PerformAlgorithmRuntimeCall(long absoluteX, long absoluteY, long absoluteZ,
                                                    int width, int height, int depth,
                                                    int arrayWidth, int arrayHeight, int arrayDepth,
                                                    int maxOffsetX, int maxOffsetY, int maxOffsetZ,
                                                    int childOffsetX, int childOffsetY, int childOffsetZ,
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
                (arrayWidth * arrayHeight * arrayDepth));

            // Depending on the argument count, invoke the method appropriately.
            switch (processCell.GetParameters().Length)
            {
                case 14: // 0 inputs
                    {

                        algorithm.Initialize(this);

                        // context, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz                    
                        for (int k = -childOffsetZ; k < (depth - childOffsetZ > 0 ? depth - childOffsetZ : 1 - childOffsetZ); k++)
                            for (int i = -childOffsetX; i < (width - childOffsetX > 0 ? width - childOffsetX : 1 - childOffsetX); i++)
                                for (int j = -childOffsetY; j < (height - childOffsetY > 0 ? height - childOffsetY : 1 - childOffsetY); j++)
                                {
                                    algorithm.ProcessCell(this, outputArray, absoluteX + i, absoluteY + j, absoluteZ + k, i, j, k, arrayWidth, arrayHeight, arrayDepth, maxOffsetX, maxOffsetY, maxOffsetZ);
                                    computations += 1;
                                }
                        break;
                    }
                case 15: // 1 input
                    {
                        // context, input, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz
                        if (this.m_Inputs[0] != null)
                        {
                            dynamic inputArray0 = this.GetInputData(0, absoluteX, absoluteY, absoluteZ, width, height, depth,
                                arrayWidth, arrayHeight, arrayDepth, maxOffsetX, maxOffsetY, maxOffsetZ,
                                childOffsetX, childOffsetY, childOffsetZ, ref computations);

                            algorithm.Initialize(this);

                            for (int k = -childOffsetZ; k < (depth - childOffsetZ > 0 ? depth - childOffsetZ : 1 - childOffsetZ); k++)
                                for (int i = -childOffsetX; i < (width - childOffsetX > 0 ? width - childOffsetX : 1 - childOffsetX); i++)
                                    for (int j = -childOffsetY; j < (height - childOffsetY > 0 ? height - childOffsetY : 1 - childOffsetY); j++)
                                    {
                                
                                        algorithm.ProcessCell(
                                    this,
                                    inputArray0,
                                    outputArray,
                                    absoluteX + i,
                                    absoluteY + j,
                                    absoluteZ + k,
                                    i,
                                    j,
                                    k,
                                    arrayWidth,
                                    arrayHeight,
                                    arrayDepth,
                                    maxOffsetX,
                                    maxOffsetY,
                                    maxOffsetZ);
                                        computations += 1;
                                    }
                        }
                        break;
                    }
                case 16: // 2 inputs
                    {
                        // context, input0, input1, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz
                        if (this.m_Inputs[0] != null && this.m_Inputs[1] != null)
                        {
                            dynamic inputArray0 = this.GetInputData(0, absoluteX, absoluteY, absoluteZ, width, height, depth,
                                arrayWidth, arrayHeight, arrayDepth, maxOffsetX, maxOffsetY, maxOffsetZ,
                                childOffsetX, childOffsetY, childOffsetZ, ref computations);

                            dynamic inputArray1 = this.GetInputData(1, absoluteX, absoluteY, absoluteZ, width, height, depth,
                                arrayWidth, arrayHeight, arrayDepth, maxOffsetX, maxOffsetY, maxOffsetZ,
                                childOffsetX, childOffsetY, childOffsetZ, ref computations);
                        
                            algorithm.Initialize(this);
                        
                            for (int k = -childOffsetZ; k < (depth - childOffsetZ > 0 ? depth - childOffsetZ : 1); k++)
                                for (int i = -childOffsetX; i < (width - childOffsetX > 0 ? width - childOffsetX : 1); i++)
                                    for (int j = -childOffsetY; j < (height - childOffsetY > 0 ? height - childOffsetY : 1); j++)
                                    {
                                
                                        algorithm.ProcessCell(
                                    this,
                                    inputArray0,
                                    inputArray1,
                                    outputArray,
                                    absoluteX + i,
                                    absoluteY + j,
                                    absoluteZ + k,
                                    i,
                                    j,
                                    k,
                                    arrayWidth,
                                    arrayHeight,
                                    arrayDepth,
                                    maxOffsetX,
                                    maxOffsetY,
                                    maxOffsetZ);
                                        computations += 1;
                                    }
                        }
                        break;
                    }
                default:
                    // FIXME!
                    throw new NotImplementedException();
            }

            if (this.DataGenerated != null)
            {
                this.DataGenerated(this, new DataGeneratedEventArgs
                {
                    Generator = this,
                    Algorithm = algorithm,
                    Data = outputArray,
                    GSAbsoluteX = absoluteX,
                    GSAbsoluteY = absoluteY,
                    GSAbsoluteZ = absoluteZ,
                    GSWidth = width,
                    GSHeight = height,
                    GSDepth = depth,
                    GSMaxOffsetX = maxOffsetX,
                    GSMaxOffsetY = maxOffsetY,
                    GSMaxOffsetZ = maxOffsetZ,
                    GSChildOffsetX = childOffsetX,
                    GSChildOffsetY = childOffsetY,
                    GSChildOffsetZ = childOffsetZ,
                    GSArrayWidth = arrayWidth,
                    GSArrayHeight = arrayHeight,
                    GSArrayDepth = arrayDepth,
                });
            }

            return outputArray;
        }

        private dynamic GetInputData(
            int idx,
            long absoluteX,
            long absoluteY,
            long absoluteZ,
            int width,
            int height,
            int depth,
            int arrayWidth,
            int arrayHeight,
            int arrayDepth,
            int maxOffsetX,
            int maxOffsetY,
            int maxOffsetZ,
            int childOffsetX,
            int childOffsetY,
            int childOffsetZ,
            ref int computations)
        {
            return this.m_Inputs[idx].PerformAlgorithmRuntimeCall(
                (this.m_Algorithm.InputWidthAtHalfSize[idx] ? ((absoluteX) < 0 ? (absoluteX - 1) / 2 : (absoluteX) / 2) : absoluteX),
                (this.m_Algorithm.InputHeightAtHalfSize[idx] ? ((absoluteY) < 0 ? (absoluteY - 1) / 2 : (absoluteY) / 2) : absoluteY),
                (this.m_Algorithm.InputDepthAtHalfSize[idx] ? ((absoluteZ) < 0 ? (absoluteZ - 1) / 2 : (absoluteZ) / 2) : absoluteZ),
                (this.m_Algorithm.InputWidthAtHalfSize[idx] ? (width / 2) + this.m_Algorithm.RequiredXBorder[idx] * 2 : width + this.m_Algorithm.RequiredXBorder[idx] * 2), 
                (this.m_Algorithm.InputHeightAtHalfSize[idx] ? (height / 2) + this.m_Algorithm.RequiredYBorder[idx] * 2 : height + this.m_Algorithm.RequiredYBorder[idx] * 2), 
                (this.m_Algorithm.InputDepthAtHalfSize[idx] ? (depth / 2) + this.m_Algorithm.RequiredZBorder[idx] * 2 : depth + this.m_Algorithm.RequiredZBorder[idx] * 2), 
                arrayWidth, 
                arrayHeight, 
                arrayDepth,
                maxOffsetX,
                maxOffsetY,
                maxOffsetZ,
                (this.m_Algorithm.InputWidthAtHalfSize[idx] ? (childOffsetX / 2) + this.m_Algorithm.RequiredXBorder[idx] : childOffsetX + this.m_Algorithm.RequiredXBorder[idx]),
                (this.m_Algorithm.InputHeightAtHalfSize[idx] ? (childOffsetY / 2) + this.m_Algorithm.RequiredYBorder[idx] : childOffsetY + this.m_Algorithm.RequiredYBorder[idx]),
                (this.m_Algorithm.InputDepthAtHalfSize[idx] ? (childOffsetZ / 2) + this.m_Algorithm.RequiredZBorder[idx] : childOffsetZ + this.m_Algorithm.RequiredZBorder[idx]),
                ref computations);
        }

        /// <summary>
        /// Generates data using the current algorithm.
        /// </summary>
        public dynamic GenerateData(long x, long y, long z, int width, int height, int depth, out int computations)
        {
            // Initialize the computation count.
            computations = 0;

            // Just replicate this into the CompiledLayer system
            int MaxOffsetX;
            int MaxOffsetY; 
            int MaxOffsetZ;
            int MaxHalfX;
            int MaxHalfY;
            int MaxHalfZ;

            FindMaximumOffsets(this, out MaxOffsetX, out MaxOffsetY, out MaxOffsetZ, out MaxHalfX, out MaxHalfY, out MaxHalfZ);
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
            // Can make this calculated by finding the total then subtracting its parents
            // for the compiled layer.

            // ix, etc = input of input layer
            // ix = x - offset (relativeX) = xFrom
            // iwidth = width * offsetX * 2.
            // iouterx = xTo

            int arrayWidth = width + MaxOffsetX * 2;
            int arrayHeight = height + MaxOffsetY * 2;
            int arrayDepth = depth + MaxOffsetZ * 2;

            dynamic resultArray = this.PerformAlgorithmRuntimeCall(
                x,
                y,
                z,
                width,
                height,
                depth,
                arrayWidth,
                arrayHeight,
                arrayDepth,
                MaxOffsetX, 
                MaxOffsetY, 
                MaxOffsetZ,
                0, 0, 0,
                ref computations);

            // Copy the result into a properly sized array.
            dynamic correctArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (width * height * depth));

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
            private set
            {
                this.m_Seed = value;
            }
        }

        /// <summary>
        /// Sets the seed of this layer and all of it's input layers
        /// recursively.
        /// </summary>
        /// <param name="seed">Seed.</param>
        public void SetSeed(long seed)
        {
            this.Seed = seed;
            if (this.m_Inputs != null)
                foreach (var input in this.m_Inputs)
                    if (input != null)
                        input.SetSeed(seed);
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

    public class DataGeneratedEventArgs : EventArgs
    {
        public IGenerator Generator;
        public Algorithm Algorithm;
        public dynamic Data;

        public long GSAbsoluteX;
        public long GSAbsoluteY;
        public long GSAbsoluteZ;
        public int GSWidth;
        public int GSHeight;
        public int GSDepth;
        public int GSMaxOffsetX;
        public int GSMaxOffsetY;
        public int GSMaxOffsetZ;
        public int GSChildOffsetX;
        public int GSChildOffsetY;
        public int GSChildOffsetZ;
        
        /// <summary>
        /// The width of the data array.
        /// </summary>
        public int GSArrayWidth;
        
        /// <summary>
        /// The height of the data array.
        /// </summary>
        public int GSArrayHeight;

        /// <summary>
        /// The depth of the data array.
        /// </summary>
        public int GSArrayDepth;
    };
    
    public delegate void DataGeneratedEventHandler(object sender,DataGeneratedEventArgs e);
}

