//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

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
        private dynamic PerformAlgorithmRuntimeCall(long xFrom, long yFrom, long zFrom,
                                                     long xTo, long yTo, long zTo,
                                                     int width, int height, int depth)
        {
            var processCell = this.m_Algorithm.GetType().GetMethod("ProcessCell");
            dynamic outputArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (int)(width * height * depth));
            switch (processCell.GetParameters().Length)
            {
                case 11:
                    {
                        // context, output, x, y, z, i, j, k, width, height, depth
                        dynamic algorithm = this.m_Algorithm;
                        for (int k = 0; k < zTo - zFrom; k++)
                            for (int i = 0; i < xTo - xFrom; i++)
                                for (int j = 0; j < yTo - yFrom; j++)
                                    algorithm.ProcessCell(this, outputArray, i + xFrom, j + yFrom, k + zFrom, i, j, k, width, height, depth);
                        break;
                    }
                case 12:
                    {
                        // context, input, output, x, y, z, i, j, k, width, height, depth
                        dynamic algorithm = this.m_Algorithm;
                        if (this.m_Inputs[0] != null)
                        {
                            dynamic inputArray = this.m_Inputs[0].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            for (int k = 0; k < zTo - zFrom; k++)
                                for (int i = 0; i < xTo - xFrom; i++)
                                    for (int j = 0; j < yTo - yFrom; j++)
                                        algorithm.ProcessCell(this, inputArray, outputArray, i + xFrom, j + yFrom, k + zFrom, i, j, k, width, height, depth);
                        }
                        break;
                    }
                case 13:
                    {
                        // context, inputA, inputB, output, x, y, z, i, j, k, width, height, depth
                        dynamic algorithm = this.m_Algorithm;
                        if (this.m_Inputs[0] != null && this.m_Inputs[1] != null)
                        {
                            dynamic inputAArray = this.m_Inputs[0].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputBArray = this.m_Inputs[1].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            for (int k = 0; k < zTo - zFrom; k++)
                                for (int i = 0; i < xTo - xFrom; i++)
                                    for (int j = 0; j < yTo - yFrom; j++)
                                        algorithm.ProcessCell(this, inputAArray, inputBArray, outputArray, i + xFrom, j + yFrom, k + zFrom, i, j, k, width, height, depth);
                        }
                        break;
                    }
                case 14:
                    {
                        // context, inputA, inputB, inputC, output, x, y, z, i, j, k, width, height, depth
                        dynamic algorithm = this.m_Algorithm;
                        if (this.m_Inputs[0] != null && this.m_Inputs[1] != null && this.m_Inputs[2] != null)
                        {
                            dynamic inputAArray = this.m_Inputs[0].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputBArray = this.m_Inputs[1].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputCArray = this.m_Inputs[2].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            for (int k = 0; k < zTo - zFrom; k++)
                                for (int i = 0; i < xTo - xFrom; i++)
                                    for (int j = 0; j < yTo - yFrom; j++)
                                        algorithm.ProcessCell(this, inputAArray, inputBArray, inputCArray, outputArray, i + xFrom, j + yFrom, k + zFrom, i, j, k, width, height, depth);
                        }
                        break;
                    }
                case 15:
                    {
                        // context, inputA, inputB, inputC, inputD, output, x, y, z, i, j, k, width, height, depth
                        dynamic algorithm = this.m_Algorithm;
                        if (this.m_Inputs[0] != null && this.m_Inputs[1] != null && this.m_Inputs[2] != null &&
                            this.m_Inputs[3] != null)
                        {
                            dynamic inputAArray = this.m_Inputs[0].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputBArray = this.m_Inputs[1].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputCArray = this.m_Inputs[2].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputDArray = this.m_Inputs[3].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            for (int k = 0; k < zTo - zFrom; k++)
                                for (int i = 0; i < xTo - xFrom; i++)
                                    for (int j = 0; j < yTo - yFrom; j++)
                                        algorithm.ProcessCell(this, inputAArray, inputBArray, inputCArray, inputDArray, outputArray, i + xFrom, j + yFrom, k + zFrom, i, j, k, width, height, depth);
                        }
                        break;
                    }
                case 15:
                    {
                        // context, inputA, inputB, inputC, inputD, inputE, output, x, y, z, i, j, k, width, height, depth
                        dynamic algorithm = this.m_Algorithm;
                        if (this.m_Inputs[0] != null && this.m_Inputs[1] != null && this.m_Inputs[2] != null &&
                            this.m_Inputs[3] != null && this.m_Inputs[4] != null)
                        {
                            dynamic inputAArray = this.m_Inputs[0].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputBArray = this.m_Inputs[1].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputCArray = this.m_Inputs[2].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputDArray = this.m_Inputs[3].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            dynamic inputEArray = this.m_Inputs[4].GenerateData(
                                xFrom - algorithm.RequiredXBorder, 
                                yFrom - algorithm.RequiredYBorder, 
                                zFrom - algorithm.RequiredZBorder, 
                                width / (algorithm.InputWidthAtHalfSize ? 2 : 1) + algorithm.RequiredXBorder * 2, 
                                height / (algorithm.InputHeightAtHalfSize ? 2 : 1) + algorithm.RequiredYBorder * 2, 
                                depth / (algorithm.InputDepthAtHalfSize ? 2 : 1) + algorithm.RequiredZBorder * 2);
                            for (int k = 0; k < zTo - zFrom; k++)
                                for (int i = 0; i < xTo - xFrom; i++)
                                    for (int j = 0; j < yTo - yFrom; j++)
                                        algorithm.ProcessCell(this, inputAArray, inputBArray, inputCArray, inputDArray, inputEArray, outputArray, i + xFrom, j + yFrom, k + zFrom, i, j, k, width, height, depth);
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
        public dynamic GenerateData(long x, long y, long z, int width, int height, int depth)
        {
            return this.PerformAlgorithmRuntimeCall(x, y, z, x + width, y + height, z + depth, width, height, depth);
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

