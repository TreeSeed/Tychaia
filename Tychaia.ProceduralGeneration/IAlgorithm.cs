// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Tychaia.ProceduralGeneration
{
    public interface IAlgorithm
    {
        /// <summary>
        /// The required amount of X border data that this algorithm
        /// needs in order to complete it's operation.
        /// </summary>
        int[] RequiredXBorder { get; }

        /// <summary>
        /// The required amount of Y border data that this algorithm
        /// needs in order to complete it's operation.
        /// </summary>
        int[] RequiredYBorder { get; }

        /// <summary>
        /// The required amount of Z border data that this algorithm
        /// needs in order to complete it's operation.
        /// </summary>
        int[] RequiredZBorder { get; }

        /// <summary>
        /// The data types that input algorithms should output for
        /// each input of this algorithm.
        /// </summary>
        Type[] InputTypes { get; }

        /// <summary>
        /// The names of the input layers; for human-readable
        /// representation.
        /// </summary>
        string[] InputNames { get; }

        /// <summary>
        /// The data type of the output of this algorithm.
        /// </summary>
        Type OutputType { get; }

        /// <summary>
        /// Whether this algorithm only requires half the width in it's inputs.
        /// </summary>
        bool[] InputWidthAtHalfSize { get; }

        /// <summary>
        /// Whether this algorithm only requires half the height in it's inputs.
        /// </summary>
        bool[] InputHeightAtHalfSize { get; }

        /// <summary>
        /// Whether this algorithm only requires half the depth in it's inputs.
        /// </summary>
        bool[] InputDepthAtHalfSize { get; }

        /// <summary>
        /// Whether this algorithm only makes sense to be represented
        /// in two-dimensions.
        /// </summary>
        bool Is2DOnly { get; }

        /// <summary>
        /// The function handler that resolves colors for
        /// values when displayed in the editor.
        /// </summary>
        Color GetColorForValue(StorageLayer parent, dynamic value);
    }

    /// These algoritms implement a ProcessCell function which is used to
    /// generate data.
    /// 
    /// The x, y, z parameters represent the current absolute location.
    /// -- This is the one to use with GetRandomNumber etc.
    /// The i, j, k parameters represent the current x, y, z location relative to the top-left corner.
    /// -- This is the one to use when reading and writing array data.
    /// The width, height, depth parameters represent the total size of the dimensions requested.
    /// -- These are also used when reading and writing array data.
    /// 
    /// The runtime context exposes random generation functionality.
    /// The output array is where you write your output, while the input
    /// arrays are where you get your data from.
    /// 
    /// IMPORTANT: The input arrays will come with border data according to
    /// the value in this.RequiredBorder.  You must use this value to offset
    /// reads from input like so:
    /// 
    /// input[(x + this.RequiredBorder) + (y + this.RequiredBorder) * width + (z + this.RequiredBorder) * width * height]

    #region Abstract Algorithm Classes
    [DataContract]
    public abstract class Algorithm : IAlgorithm
    {
        [Category("Base")]
        public virtual int[] RequiredXBorder
        {
            get
            {
                return new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                };
            }
        }

        [Category("Base")]
        public virtual int[] RequiredYBorder
        {
            get
            {
                return new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                };
            }
        }

        [Category("Base")]
        public virtual int[] RequiredZBorder
        {
            get
            {
                return new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                };
            }
        }

        [Category("Base")]
        public virtual bool[] InputWidthAtHalfSize
        {
            get
            {
                return new[]
                {
                    false,
                    false,
                    false,
                    false,
                    false,
                    false
                };
            }
        }

        [Category("Base")]
        public virtual bool[] InputHeightAtHalfSize
        {
            get
            {
                return new[]
                {
                    false,
                    false,
                    false,
                    false,
                    false,
                    false
                };
            }
        }

        [Category("Base")]
        public virtual bool[] InputDepthAtHalfSize
        {
            get
            {
                return new[]
                {
                    false,
                    false,
                    false,
                    false,
                    false,
                    false
                };
            }
        }

        [Category("Base")]
        public abstract Type[] InputTypes { get; }

        [Category("Base")]
        public abstract string[] InputNames { get; }

        [Category("Base")]
        public abstract Type OutputType { get; }

        [Category("Base")]
        public abstract bool Is2DOnly { get; }

        public abstract Color GetColorForValue(StorageLayer parent, dynamic value);

        public virtual void Initialize(IRuntimeContext context)
        {
        }

        /// <summary>
        /// Delegates the color logic to one of the parents of this algorithm.
        /// </summary>
        /// <param name="layer">The storage layer holding this algorithm.</param>
        /// <param name="value">The value to determine.</param>
        /// <param name="index">The index of the parent to delegate to.</param>
        /// <returns>The color determined by the parent.</returns>
        public Color DelegateColorForValueToParent(StorageLayer layer, dynamic value, int index = 0)
        {
            if (layer.Inputs[index] == null)
                return Color.Gray;
            return layer.Inputs[index].Algorithm.GetColorForValue(layer.Inputs[index], value);
        }
    }

    [DataContract]
    public abstract class Algorithm<TOutput> : Algorithm
    {
        public override sealed Type[] InputTypes
        {
            get { return Type.EmptyTypes; }
        }

        [ReadOnly(true)]
        public override sealed string[] InputNames
        {
            get { return new string[] { }; }
        }

        public override sealed Type OutputType
        {
            get { return typeof(TOutput); }
        }

        public abstract void ProcessCell(IRuntimeContext context,
            TOutput[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz);
    }

    [DataContract]
    public abstract class Algorithm<TInput, TOutput> : Algorithm
    {
        public override sealed Type[] InputTypes
        {
            get { return new[] { typeof(TInput) }; }
        }

        public override sealed Type OutputType
        {
            get { return typeof(TOutput); }
        }

        public abstract void ProcessCell(IRuntimeContext context,
            TInput[] input,
            TOutput[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz);
    }

    [DataContract]
    public abstract class Algorithm<TInputA, TInputB, TOutput> : Algorithm
    {
        public override sealed Type[] InputTypes
        {
            get { return new[] { typeof(TInputA), typeof(TInputB) }; }
        }

        public override sealed Type OutputType
        {
            get { return typeof(TOutput); }
        }

        public abstract void ProcessCell(IRuntimeContext context,
            TInputA[] inputA,
            TInputB[] inputB,
            TOutput[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz);
    }

    [DataContract]
    public abstract class Algorithm<TInputA, TInputB, TInputC, TOutput> : Algorithm
    {
        public override sealed Type[] InputTypes
        {
            get
            {
                return new[]
                {
                    typeof(TInputA),
                    typeof(TInputB),
                    typeof(TInputC)
                };
            }
        }

        public override sealed Type OutputType
        {
            get { return typeof(TOutput); }
        }

        public abstract void ProcessCell(IRuntimeContext context,
            TInputA[] inputA,
            TInputB[] inputB,
            TInputC[] inputC,
            TOutput[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz);
    }

    [DataContract]
    public abstract class Algorithm<TInputA, TInputB, TInputC, TInputD, TOutput> : Algorithm
    {
        public override sealed Type[] InputTypes
        {
            get
            {
                return new[]
                {
                    typeof(TInputA),
                    typeof(TInputB),
                    typeof(TInputC),
                    typeof(TInputD)
                };
            }
        }

        public override sealed Type OutputType
        {
            get { return typeof(TOutput); }
        }

        public abstract void ProcessCell(IRuntimeContext context,
            TInputA[] inputA,
            TInputB[] inputB,
            TInputC[] inputC,
            TInputD[] inputD,
            TOutput[] output,
            long x,
            long y,
            long z,
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz);
    }

    [DataContract]
    public abstract class Algorithm<TInputA, TInputB, TInputC, TInputD, TInputE, TOutput> : Algorithm
    {
        public override sealed Type[] InputTypes
        {
            get
            {
                return new[]
                {
                    typeof(TInputA),
                    typeof(TInputB),
                    typeof(TInputC),
                    typeof(TInputD),
                    typeof(TInputE)
                };
            }
        }

        public override sealed Type OutputType
        {
            get { return typeof(TOutput); }
        }

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
            int i,
            int j,
            int k,
            int width,
            int height,
            int depth,
            int ox,
            int oy,
            int oz);
    }

    #endregion
}
