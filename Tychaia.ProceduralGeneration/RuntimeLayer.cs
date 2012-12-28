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
        private IAlgorithm[]
            m_Inputs;

        /// <summary>
        /// The current algorithm that this runtime layer is using.
        /// </summary>
        internal IAlgorithm Algorithm
        {
            get { return this.m_Algorithm; }
        }

        /// <summary>
        /// Creates a new runtime layer that holds the specified algorithm.
        /// </summary>
        public RuntimeLayer(IAlgorithm algorithm)
        {
            this.m_Algorithm = algorithm;
            var inputs = new List<IAlgorithm>();
            for (var i = 0; i < algorithm.InputTypes.Length; i++)
                inputs.Add(null);
            this.m_Inputs = inputs.ToArray();
        }

        /// <summary>
        /// Determines whether or not the specified input algorithm can be used as an
        /// input for the current algorithm in the specified index slot.
        /// </summary>
        public bool CanBeInput(int index, IAlgorithm input)
        {
            if (index < 0 || index >= this.m_Algorithm.InputTypes.Length)
                return false;
            return (input.OutputType == this.m_Algorithm.InputTypes[index]);
        }

        /// <summary>
        /// Sets the specified algorithm as the input at the specified index.
        /// </summary>
        public void SetInput(int index, IAlgorithm input)
        {
            if (!this.CanBeInput(index, input))
                throw new InvalidOperationException("Specified algorithm can not be set as input at this index.");
            this.m_Inputs[index] = input;
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
            /*
            var processCell = this.m_Algorithm.GetType().GetMethod("ProcessCell");
            var parameters = new List<object>();
            dynamic outputArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (int)(width * height * depth));
            parameters.Add(this);
            parameters.Add(outputArray);
            for (var i = 0; i < this.m_Inputs.Length; i++)
            {
                parameters.Add(Activator.CreateInstance(
                    this.m_Algorithm.InputTypes[i].MakeArrayType(),
                    (width + this.m_Algorithm.RequiredBorder) *
                    (height + this.m_Algorithm.RequiredBorder) *
                    (depth + this.m_Algorithm.RequiredBorder)));
            }
            parameters.Add(xFrom);
            parameters.Add(yFrom);
            parameters.Add(zFrom);
            parameters.Add(width);
            parameters.Add(height);
            parameters.Add(depth);
            if (processCell.GetParameters().Length != parameters.Count)
                throw new InvalidOperationException("parameter count mismatch on performing call");
            for (var p = 0; p < parameters.Count; p++)
            {
                if (!processCell.GetParameters()[p].ParameterType.IsAssignableFrom(parameters[p].GetType()))
                    throw new InvalidOperationException("parameter '" + processCell.GetParameters()[p].Name + "' can't be assigned to... " +
                        "expected " + processCell.GetParameters()[p].ParameterType + " " + 
                        "got " + parameters[p].GetType()); 
            }
            for (var i = xFrom; i != xTo; i++)
                for (var j = yFrom; j != yTo; j++)
                    for (var k = zFrom; k != zTo; k++)
                    {
                        parameters[parameters.Count - 6] = i;
                        parameters[parameters.Count - 5] = j;
                        parameters[parameters.Count - 4] = k;
                        processCell.Invoke(this.m_Algorithm, parameters.ToArray());
                    }
            return outputArray as object[];
            */
            var processCell = this.m_Algorithm.GetType().GetMethod("ProcessCell");
            dynamic outputArray = Activator.CreateInstance(
                this.m_Algorithm.OutputType.MakeArrayType(),
                (int)(width * height * depth));
            switch (processCell.GetParameters().Length)
            {
                case 8:
                    // context, output, x, y, z, width, height, depth
                    dynamic algorithm = this.m_Algorithm;
                    for (var i = xFrom; i != xTo; i++)
                        for (var j = yFrom; j != yTo; j++)
                            for (var k = zFrom; k != zTo; k++)
                                algorithm.ProcessCell(this, outputArray, i, j, k, width, height, depth);
                    break;
                default:
                    // FIXME!
                    throw new NotImplementedException();
            }
            return outputArray;
        }
        
        /// <summary>
        /// Generates data using the current algorithm.
        /// </summary>
        public dynamic GenerateData(long x, long y, int width, int height)
        {
            return this.PerformAlgorithmRuntimeCall(x, y, 0, x + width, y + height, 1, width, height, 1);
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
            unchecked
            {
                int a = this.GetRandomInt(x, y, z, modifier);
                if (a < 0)
                    a += int.MaxValue;
                return a % end;
            }
        }
        
        /// <summary>
        /// Returns a random positive integer between the specified inclusive start
        /// value and the exclusive end value.
        /// </summary>
        public int GetRandomRange(long x, long y, long z, int start, int end, long modifier)
        {
            unchecked
            {
                int a = this.GetRandomInt(x, y, z, modifier);
                if (a < 0)
                    a += int.MaxValue;
                return a % (end - start) + start;
            }
        }
        
        /// <summary>
        /// Returns a random integer over the range of valid integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        public int GetRandomInt(long x, long y, long z, long modifier = 0)
        {
            unchecked
            {
                return (int)(this.GetRandomNumber(x, y, z, modifier) % int.MaxValue);
            }
        }
        
        /// <summary>
        /// Returns a random long integer over the range of valid long integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        public long GetRandomLong(long x, long y, long z, long modifier = 0)
        {
            return this.GetRandomNumber(x, y, z, modifier);
        }
        
        /// <summary>
        /// Returns a random double between the range of 0.0 and 1.0 based on
        /// the provided X and Y position, and the specified modifier.
        /// </summary>
        public double GetRandomDouble(long x, long y, long z, long modifier = 0)
        {
            long a = this.GetRandomNumber(x, y, z, modifier) / 2;
            if (a < 0)
                a += long.MaxValue;
            return (double)a / (double)long.MaxValue;
        }
        
        private long GetRandomNumber(long x, long y, long z, long modifier)
        {
            /* From: http://stackoverflow.com/questions/2890040/implementing-gethashcode
             * Although we aren't implementing GetHashCode, it's still a good way to generate
             * a unique number given a limited set of fields */
            unchecked
            {
                long seed = (x - 1) * 3661988493967 + (y - 1);
                seed += (x - 2) * 2990430311017;
                seed *= (y - 3) * 14475080218213;
                seed += modifier;
                seed += (y - 4) * 28124722524383;
                seed += (z - 5) * 25905201761893;
                seed *= (x - 6) * 16099760261113;
                seed += (x - 7) * this.m_Seed;
                seed *= (y - 8) * this.m_Seed;
                seed += (z - 9) * 55497960863;
                seed *= (z - 10) * 611286883423;
                seed += modifier;
                // Prevents the seed from being 0 along an axis.
                seed += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;

                long rng = seed * seed;
                rng += (x - 11) * 2990430311017;
                rng *= (y - 12) * 14475080218213;
                rng *= (z - 13) * 23281823741513;
                rng -= seed * 28124722524383;
                rng *= (x - 14) * 16099760261113;
                rng += seed * this.m_Seed;
                rng *= (y - 15) * this.m_Seed;
                rng *= (z - 16) * 18193477834921;
                rng += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;
                rng += modifier;
                rng += 3661988493967;
                
                return rng;
            }
        }

        #endregion
    }
}

