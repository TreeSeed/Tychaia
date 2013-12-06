// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Tychaia.Globals;

namespace Tychaia.ProceduralGeneration
{
    [NoInstrumentation]
    public class RuntimeLayer : IRuntimeContext, IGenerator
    {
        /// <summary>
        /// Arbitrary userdata to associate with this object.
        /// </summary>
        public object Userdata;

        /// <summary>
        /// The current algorithm for this layer.
        /// </summary>
        [DataMember] private IAlgorithm
            m_Algorithm;

        /// <summary>
        /// The input layers.
        /// </summary>
        [DataMember] private RuntimeLayer[]
            m_Inputs;

        private long m_Seed;
        private readonly IArrayPool m_ArrayPool;
        private readonly IPool m_Pool;

        /// <summary>
        /// Creates a new runtime layer that holds the specified algorithm.
        /// </summary>
        public RuntimeLayer(
            IPool pool,
            IArrayPool arrayPool,
            IAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");
            this.m_Pool = pool;
            this.m_ArrayPool = arrayPool;
            this.m_Algorithm = algorithm;
            var inputs = new List<RuntimeLayer>();
            for (var i = 0; i < algorithm.InputTypes.Length; i++)
                inputs.Add(null);
            this.m_Inputs = inputs.ToArray();

            this.Seed = 0;
            this.Modifier = 0;
        }

        /// <summary>
        /// The current algorithm that this runtime layer is using.
        /// </summary>
        public IAlgorithm Algorithm
        {
            get { return this.m_Algorithm; }
        }

        /// <summary>
        /// Generates data using the current algorithm.
        /// </summary>
        public dynamic GenerateData(long x, long y, long z, int width, int height, int depth, out int computations)
        {
            // Initialize the computation count.
            computations = 0;

            // Just replicate this into the CompiledLayer system
            var MaxOffsetX = 0;
            var MaxOffsetY = 0;
            var MaxOffsetZ = 0;

            FindMaximumOffsets(this, ref MaxOffsetX, ref MaxOffsetY, ref MaxOffsetZ);

            var arrayWidth = width + (MaxOffsetX * 2);
            var arrayHeight = height + (MaxOffsetY * 2);
            var arrayDepth = depth + (MaxOffsetZ * 2);

            this.m_Pool.Begin();
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
                0, 
                0, 
                0,
                ref computations);

            // Copy the result into a properly sized array.
            dynamic correctArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                width * height * depth);

            for (var k = 0; k < depth; k++)
                for (var i = 0; i < width; i++)
                    for (var j = 0; j < height; j++)
                        correctArray[i + (j * width) + (k * width * height)] =
                            resultArray[(i + MaxOffsetX) +
                                        ((j + MaxOffsetY) * arrayWidth) +
                                        ((k + MaxOffsetZ) * arrayWidth * arrayHeight)];
            this.m_Pool.End();

            // Return the result.
            return correctArray;
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
        /// The modifier used by algorithms as an additional input to the
        /// random function calls.
        /// </summary>
        [DataMember]
        [Description("The seed modifier value to apply.")]
        public long Modifier { get; set; }

        /// <summary>
        /// The world seed.
        /// </summary>
        public long Seed
        {
            get { return this.m_Seed; }
            private set { this.m_Seed = value; }
        }

        /// <summary>
        /// Occurs when data has been generated for an algorithm.
        /// </summary>
        public event DataGeneratedEventHandler DataGenerated;

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
            if (this.m_Algorithm.Is2DOnly && !input.m_Algorithm.Is2DOnly)
                return false;
            return input.m_Algorithm.OutputType == this.m_Algorithm.InputTypes[index];
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

        // Just finding offsets, then use them to determine max width, start X location, etc.
        public static void FindMaximumOffsets(
            RuntimeLayer layer,
            ref int offsetX,
            ref int offsetY,
            ref int offsetZ)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            var offsetSaveX = offsetX;
            var offsetSaveY = offsetY;
            var offsetSaveZ = offsetZ;

            if (layer.m_Inputs.Length != 0)
            {
                var inputs = 0;
                var TempOffsetX = new int[layer.m_Inputs.Length];
                var TempOffsetY = new int[layer.m_Inputs.Length];
                var TempOffsetZ = new int[layer.m_Inputs.Length];

                foreach (var input in layer.m_Inputs)
                {
                    if (input == null)
                        continue;

                    offsetX = offsetSaveX;
                    offsetY = offsetSaveY;
                    offsetZ = offsetSaveZ;

                    bool x = false;
                    bool y = false;
                    bool z = false;

                    // TODO: Fix this so that it gets if the input is at half then get the full offset of this and 1/2 of all of the parents.
                    // Need to make this so that it gets all of the offset then scales it based off of how many times it is halved.
                    if (layer.m_Algorithm.InputWidthAtHalfSize[inputs] && !x && offsetX != 0)
                    {
                        offsetX = (offsetX + 1) / 2;
                        x = true;
                    }

                    if (layer.m_Algorithm.InputHeightAtHalfSize[inputs] && !y && offsetY != 0)
                    {
                        offsetY = (offsetY + 1) / 2;
                        y = true;
                    }

                    if (layer.m_Algorithm.InputDepthAtHalfSize[inputs] && !z && offsetZ != 0)
                    {
                        offsetZ = (offsetZ + 1) / 2;
                        z = true;
                    }

                    FindMaximumOffsets(input, ref offsetX, ref offsetY, ref offsetZ);

                    TempOffsetX[inputs] = offsetX + Math.Abs(layer.m_Algorithm.RequiredXBorder[inputs]);
                    TempOffsetY[inputs] = offsetY + Math.Abs(layer.m_Algorithm.RequiredYBorder[inputs]);
                    TempOffsetZ[inputs] = offsetZ + Math.Abs(layer.m_Algorithm.RequiredZBorder[inputs]);
                    inputs++;
                }

                for (var count = 0; count < inputs; count++)
                {
                    if (offsetX < TempOffsetX[count])
                        offsetX = TempOffsetX[count];
                    if (offsetY < TempOffsetY[count])
                        offsetY = TempOffsetY[count];
                    if (offsetZ < TempOffsetZ[count])
                        offsetZ = TempOffsetZ[count];
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
        private dynamic PerformAlgorithmRuntimeCall(
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
            // Get the method for processing cells.
            dynamic algorithm = this.m_Algorithm;
            var processCell = this.m_Algorithm.GetType().GetMethod("ProcessCell");

            dynamic outputArray = this.m_Pool.Get(
                this.m_Algorithm.OutputType.MakeArrayType(),
                arrayWidth * arrayHeight * arrayDepth);

            var iEnd = width - childOffsetX > 0 ? width - childOffsetX : 1 - childOffsetX;
            var jEnd = height - childOffsetY > 0 ? height - childOffsetY : 1 - childOffsetY;
            var kEnd = depth - childOffsetZ > 0 ? depth - childOffsetZ : 1 - childOffsetZ;

            // Depending on the argument count, invoke the method appropriately.
            switch (processCell.GetParameters().Length)
            {
                case 14: // 0 inputs
                {
                    algorithm.Initialize(this);

                    // context, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz
                    for (var k = -childOffsetZ; k < kEnd; k++)
                        for (var i = -childOffsetX; i < iEnd; i++)
                            for (var j = -childOffsetY; j < jEnd; j++)
                            {
                                if (this.m_Algorithm.Is2DOnly && k != -childOffsetZ)
                                {
                                    outputArray[i + maxOffsetX +
                                        ((j + maxOffsetY) * arrayWidth) +
                                        ((k + maxOffsetZ) * arrayWidth * arrayHeight)] =
                                        outputArray[i + maxOffsetX +
                                            ((j + maxOffsetY) * arrayWidth) +
                                            ((-childOffsetZ + maxOffsetZ) * arrayWidth * arrayHeight)];
                                    continue;
                                }

                                algorithm.ProcessCell(
                                    this,
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

                    break;
                }

                case 15: // 1 input
                {
                    // context, input, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz
                    if (this.m_Inputs[0] != null)
                    {
                        dynamic inputArray0;
                        if (algorithm.InputIs2D[0])
                            inputArray0 = this.GetInputData(
                                0, 
                                absoluteX, 
                                absoluteY, 
                                0, 
                                width, 
                                height, 
                                1,
                                arrayWidth, 
                                arrayHeight, 
                                1, 
                                maxOffsetX, 
                                maxOffsetY, 
                                0,
                                childOffsetX, 
                                childOffsetY, 
                                0, 
                                ref computations);
                        else
                            inputArray0 = this.GetInputData(
                                0, 
                                absoluteX, 
                                absoluteY, 
                                absoluteZ, 
                                width, 
                                height, 
                                depth,
                                arrayWidth, 
                                arrayHeight, 
                                arrayDepth, 
                                maxOffsetX, 
                                maxOffsetY, 
                                maxOffsetZ,
                                childOffsetX, 
                                childOffsetY, 
                                childOffsetZ, 
                                ref computations);

                        algorithm.Initialize(this);

                        for (var k = -childOffsetZ; k < kEnd; k++)
                            for (var i = -childOffsetX; i < iEnd; i++)
                                for (var j = -childOffsetY; j < jEnd; j++)
                                {
                                    if (this.m_Algorithm.Is2DOnly && k != -childOffsetZ)
                                    {
                                        outputArray[i + maxOffsetX +
                                            ((j + maxOffsetY) * arrayWidth) +
                                            ((k + maxOffsetZ) * arrayWidth * arrayHeight)] =
                                            outputArray[i + maxOffsetX +
                                                ((j + maxOffsetY) * arrayWidth) +
                                                ((-childOffsetZ + maxOffsetZ) * arrayWidth * arrayHeight)];
                                        continue;
                                    }
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
                        dynamic inputArray0, inputArray1;
                        if (algorithm.InputIs2D[0])
                            inputArray0 = this.GetInputData(
                                0,
                                absoluteX,
                                absoluteY,
                                0,
                                width,
                                height,
                                1,
                                arrayWidth,
                                arrayHeight,
                                1,
                                maxOffsetX,
                                maxOffsetY,
                                0,
                                childOffsetX,
                                childOffsetY,
                                0,
                                ref computations);
                        else
                            inputArray0 = this.GetInputData(
                                0,
                                absoluteX,
                                absoluteY,
                                absoluteZ,
                                width,
                                height,
                                depth,
                                arrayWidth,
                                arrayHeight,
                                arrayDepth,
                                maxOffsetX,
                                maxOffsetY,
                                maxOffsetZ,
                                childOffsetX,
                                childOffsetY,
                                childOffsetZ,
                                ref computations);
                        if (algorithm.InputIs2D[1])
                            inputArray1 = this.GetInputData(
                                1,
                                absoluteX,
                                absoluteY,
                                0,
                                width,
                                height,
                                1,
                                arrayWidth,
                                arrayHeight,
                                1,
                                maxOffsetX,
                                maxOffsetY,
                                0,
                                childOffsetX,
                                childOffsetY,
                                0,
                                ref computations);
                        else
                            inputArray1 = this.GetInputData(
                                1,
                                absoluteX,
                                absoluteY,
                                absoluteZ,
                                width,
                                height,
                                depth,
                                arrayWidth,
                                arrayHeight,
                                arrayDepth,
                                maxOffsetX,
                                maxOffsetY,
                                maxOffsetZ,
                                childOffsetX,
                                childOffsetY,
                                childOffsetZ,
                                ref computations);

                        algorithm.Initialize(this);

                        for (var k = -childOffsetZ; k < kEnd; k++)
                            for (var i = -childOffsetX; i < iEnd; i++)
                                for (var j = -childOffsetY; j < jEnd; j++)
                                {
                                    if (this.m_Algorithm.Is2DOnly && k != -childOffsetZ)
                                    {
                                        outputArray[i + maxOffsetX +
                                            ((j + maxOffsetY) * arrayWidth) +
                                            ((k + maxOffsetZ) * arrayWidth * arrayHeight)] =
                                            outputArray[i + maxOffsetX +
                                                ((j + maxOffsetY) * arrayWidth) +
                                                ((-childOffsetZ + maxOffsetZ) * arrayWidth * arrayHeight)];
                                        continue;
                                    }

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

                case 17: // 3 inputs
                {
                    // context, input0, input1, input2, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz
                    if (this.m_Inputs[0] != null && this.m_Inputs[1] != null && this.m_Inputs[2] != null)
                    {
                        dynamic inputArray0, inputArray1, inputArray2;
                        if (algorithm.InputIs2D[0])
                            inputArray0 = this.GetInputData(
                                0,
                                absoluteX,
                                absoluteY,
                                0,
                                width,
                                height,
                                1,
                                arrayWidth,
                                arrayHeight,
                                1,
                                maxOffsetX,
                                maxOffsetY,
                                0,
                                childOffsetX,
                                childOffsetY,
                                0,
                                ref computations);
                        else
                            inputArray0 = this.GetInputData(
                                0,
                                absoluteX,
                                absoluteY,
                                absoluteZ,
                                width,
                                height,
                                depth,
                                arrayWidth,
                                arrayHeight,
                                arrayDepth,
                                maxOffsetX,
                                maxOffsetY,
                                maxOffsetZ,
                                childOffsetX,
                                childOffsetY,
                                childOffsetZ,
                                ref computations);
                        if (algorithm.InputIs2D[1])
                            inputArray1 = this.GetInputData(
                                1,
                                absoluteX,
                                absoluteY,
                                0,
                                width,
                                height,
                                1,
                                arrayWidth,
                                arrayHeight,
                                1,
                                maxOffsetX,
                                maxOffsetY,
                                0,
                                childOffsetX,
                                childOffsetY,
                                0,
                                ref computations);
                        else
                            inputArray1 = this.GetInputData(
                                1,
                                absoluteX,
                                absoluteY,
                                absoluteZ,
                                width,
                                height,
                                depth,
                                arrayWidth,
                                arrayHeight,
                                arrayDepth,
                                maxOffsetX,
                                maxOffsetY,
                                maxOffsetZ,
                                childOffsetX,
                                childOffsetY,
                                childOffsetZ,
                                ref computations);
                        if (algorithm.InputIs2D[2])
                            inputArray2 = this.GetInputData(
                                2,
                                absoluteX,
                                absoluteY,
                                0,
                                width,
                                height,
                                1,
                                arrayWidth,
                                arrayHeight,
                                1,
                                maxOffsetX,
                                maxOffsetY,
                                0,
                                childOffsetX,
                                childOffsetY,
                                0,
                                ref computations);
                        else
                            inputArray2 = this.GetInputData(
                                2,
                                absoluteX,
                                absoluteY,
                                absoluteZ,
                                width,
                                height,
                                depth,
                                arrayWidth,
                                arrayHeight,
                                arrayDepth,
                                maxOffsetX,
                                maxOffsetY,
                                maxOffsetZ,
                                childOffsetX,
                                childOffsetY,
                                childOffsetZ,
                                ref computations);

                        algorithm.Initialize(this);

                        for (var k = -childOffsetZ; k < kEnd; k++)
                            for (var i = -childOffsetX; i < iEnd; i++)
                                for (var j = -childOffsetY; j < jEnd; j++)
                                {
                                    if (this.m_Algorithm.Is2DOnly && k != -childOffsetZ)
                                    {
                                        outputArray[i + maxOffsetX +
                                            ((j + maxOffsetY) * arrayWidth) +
                                            ((k + maxOffsetZ) * arrayWidth * arrayHeight)] =
                                            outputArray[i + maxOffsetX +
                                                ((j + maxOffsetY) * arrayWidth) +
                                                ((-childOffsetZ + maxOffsetZ) * arrayWidth * arrayHeight)];
                                        continue;
                                    }

                                    algorithm.ProcessCell(
                                        this,
                                        inputArray0,
                                        inputArray1,
                                        inputArray2,
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
                (this.m_Algorithm.InputWidthAtHalfSize[idx]
                    ? (absoluteX < 0 ? (absoluteX - 1) / 2 : absoluteX / 2)
                    : absoluteX),
                (this.m_Algorithm.InputHeightAtHalfSize[idx]
                    ? (absoluteY < 0 ? (absoluteY - 1) / 2 : absoluteY / 2)
                    : absoluteY),
                (this.m_Algorithm.InputDepthAtHalfSize[idx]
                    ? (absoluteZ < 0 ? (absoluteZ - 1) / 2 : absoluteZ / 2)
                    : absoluteZ),
                (this.m_Algorithm.InputWidthAtHalfSize[idx]
                    ? ((width + 1) / 2) + this.m_Algorithm.RequiredXBorder[idx] * 2
                    : width + (this.m_Algorithm.RequiredXBorder[idx] * 2)),
                (this.m_Algorithm.InputHeightAtHalfSize[idx]
                    ? ((height + 1) / 2) + this.m_Algorithm.RequiredYBorder[idx] * 2
                    : height + (this.m_Algorithm.RequiredYBorder[idx] * 2)),
                (this.m_Algorithm.InputDepthAtHalfSize[idx]
                    ? ((depth + 1) / 2) + this.m_Algorithm.RequiredZBorder[idx] * 2
                    : depth + (this.m_Algorithm.RequiredZBorder[idx] * 2)),
                arrayWidth,
                arrayHeight,
                arrayDepth,
                maxOffsetX,
                maxOffsetY,
                maxOffsetZ,
                (this.m_Algorithm.InputWidthAtHalfSize[idx]
                    ? ((childOffsetX + 1) / 2) + this.m_Algorithm.RequiredXBorder[idx]
                    : childOffsetX + this.m_Algorithm.RequiredXBorder[idx]),
                (this.m_Algorithm.InputHeightAtHalfSize[idx]
                    ? ((childOffsetY + 1) / 2) + this.m_Algorithm.RequiredYBorder[idx]
                    : childOffsetY + this.m_Algorithm.RequiredYBorder[idx]),
                (this.m_Algorithm.InputDepthAtHalfSize[idx]
                    ? ((childOffsetZ + 1) / 2) + this.m_Algorithm.RequiredZBorder[idx]
                    : childOffsetZ + this.m_Algorithm.RequiredZBorder[idx]),
                ref computations);
        }
    }

    public class DataGeneratedEventArgs : EventArgs
    {
        public Algorithm Algorithm;
        public dynamic Data;

        public long GSAbsoluteX;
        public long GSAbsoluteY;
        public long GSAbsoluteZ;

        /// <summary>
        /// The depth of the data array.
        /// </summary>
        public int GSArrayDepth;

        /// <summary>
        /// The height of the data array.
        /// </summary>
        public int GSArrayHeight;

        /// <summary>
        /// The width of the data array.
        /// </summary>
        public int GSArrayWidth;

        public int GSChildOffsetX;
        public int GSChildOffsetY;
        public int GSChildOffsetZ;

        public int GSDepth;
        public int GSHeight;
        public int GSMaxOffsetX;
        public int GSMaxOffsetY;
        public int GSMaxOffsetZ;
        public int GSWidth;
        public IGenerator Generator;
    };

    public delegate void DataGeneratedEventHandler(object sender, DataGeneratedEventArgs e);
}
