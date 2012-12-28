//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Tychaia.ProceduralGeneration
{
    public interface IAlgorithm
    {
        /// <summary>
        /// The required amount of border data that this algorithm
        /// needs in order to complete it's operation.
        /// </summary>
        int RequiredBorder { get; }

        /// <summary>
        /// The data types that input algorithms should output for
        /// each input of this algorithm.
        /// </summary>
        Type[] InputTypes { get; }

        /// <summary>
        /// The data type of the output of this algorithm.
        /// </summary>
        Type OutputType { get; }
    }

    #region Abstract Algorithm Classes

    public abstract class Algorithm<TOutput> : IAlgorithm
    {
        public abstract void ProcessCell(IRuntimeContext context,
                                         TOutput[] output,
                                         long x,
                                         long y,
                                         long z,
                                         int width,
                                         int height,
                                         int depth);

        public abstract int RequiredBorder { get; }

        public Type[] InputTypes
        {
            get { return new Type[] { }; }
        }

        public Type OutputType
        {
            get { return typeof(TOutput); }
        }
    }

    public abstract class Algorithm<TInput, TOutput>
    {
        public abstract void ProcessCell(IRuntimeContext context,
                                         TInput[] input,
                                         TOutput[] output,
                                         long x,
                                         long y,
                                         long z,
                                         int width,
                                         int height,
                                         int depth);
        
        public abstract int RequiredBorder { get; }

        public Type[] InputTypes
        {
            get { return new Type[] { typeof(TInput) }; }
        }
        
        public Type OutputType
        {
            get { return typeof(TOutput); }
        }
    }
    
    public abstract class Algorithm<TInputA, TInputB, TOutput>
    {
        public abstract void ProcessCell(IRuntimeContext context,
                                         TInputA[] inputA,
                                         TInputB[] inputB,
                                         TOutput[] output,
                                         long x,
                                         long y,
                                         long z,
                                         int width,
                                         int height,
                                         int depth);

        public abstract int RequiredBorder { get; }

        public Type[] InputTypes
        {
            get { return new Type[] { typeof(TInputA), typeof(TInputB) }; }
        }
        
        public Type OutputType
        {
            get { return typeof(TOutput); }
        }
    }
    
    public abstract class Algorithm<TInputA, TInputB, TInputC, TOutput>
    {
        public abstract void ProcessCell(IRuntimeContext context,
                                         TInputA[] inputA,
                                         TInputB[] inputB,
                                         TInputC[] inputC,
                                         TOutput[] output,
                                         long x,
                                         long y,
                                         long z,
                                         int width,
                                         int height,
                                         int depth);
        
        public abstract int RequiredBorder { get; }
        
        public Type[] InputTypes
        {
            get
            {
                return new Type[]
                {
                    typeof(TInputA),
                    typeof(TInputB),
                    typeof(TInputC)
                };
            }
        }
        
        public Type OutputType
        {
            get { return typeof(TOutput); }
        }
    }
    
    public abstract class Algorithm<TInputA, TInputB, TInputC, TInputD, TOutput>
    {
        public abstract void ProcessCell(IRuntimeContext context,
                                         TInputA[] inputA,
                                         TInputB[] inputB,
                                         TInputC[] inputC,
                                         TInputD[] inputD,
                                         TOutput[] output,
                                         long x,
                                         long y,
                                         long z,
                                         int width,
                                         int height,
                                         int depth);
        
        public abstract int RequiredBorder { get; }
        
        public Type[] InputTypes
        {
            get
            {
                return new Type[]
                {
                    typeof(TInputA),
                    typeof(TInputB),
                    typeof(TInputC),
                    typeof(TInputD)
                };
            }
        }
        
        public Type OutputType
        {
            get { return typeof(TOutput); }
        }
    }
    
    public abstract class Algorithm<TInputA, TInputB, TInputC, TInputD, TInputE, TOutput>
    {
        public abstract void ProcessCell(IRuntimeContext context,
                                         TInputA[] inputA,
                                         TInputB[] inputB,
                                         TInputC[] inputC,
                                         TInputD[] inputD,
                                         TInputE[] inputE,
                                         TOutput[] output,
                                         long x,
                                         long y,
                                         long z,
                                         int width,
                                         int height,
                                         int depth);
        
        public abstract int RequiredBorder { get; }
        
        public Type[] InputTypes
        {
            get
            {
                return new Type[]
                {
                    typeof(TInputA),
                    typeof(TInputB),
                    typeof(TInputC),
                    typeof(TInputD),
                    typeof(TInputE)
                };
            }
        }
        
        public Type OutputType
        {
            get { return typeof(TOutput); }
        }
    }

    #endregion
}

