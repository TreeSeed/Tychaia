//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;

namespace Tychaia.ProceduralGeneration.Compiler
{
    /// <summary>
    /// Contains the start and end loop values at and the layer which is activated
    /// when they become true.
    /// </summary>
    public class RangedLayer
    {
        public Expression X { get; set; }
        public Expression Y { get; set; }
        public Expression Z { get; set; }
        public Expression Width { get; set; }
        public Expression Height { get; set; }
        public Expression Depth { get; set; }
        public RuntimeLayer Layer { get; set; }
        public RangedLayer[] Inputs { get; set; }

        // Just calculating offsets
        public Expression OffsetX { get; set; }
        public Expression OffsetY { get; set; }
        public Expression OffsetZ { get; set; }

        public Expression OuterX
        {
            get
            {
                return new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                    new ParenthesizedExpression(this.Width.Clone()),
                    BinaryOperatorType.Subtract,
                    new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                    new IdentifierExpression("x"),
                    BinaryOperatorType.Subtract,
                    new ParenthesizedExpression(this.X.Clone())
                ))));
            }
        }

        public Expression OuterY
        {
            get
            {
                return new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                    new ParenthesizedExpression(this.Height.Clone()),
                    BinaryOperatorType.Subtract,
                    new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                    new IdentifierExpression("y"),
                    BinaryOperatorType.Subtract,
                    new ParenthesizedExpression(this.Y.Clone())
                ))));
            }
        }
        
        public Expression OuterZ
        {
            get
            {
                return new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                    new ParenthesizedExpression(this.Depth.Clone()),
                    BinaryOperatorType.Subtract,
                    new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                    new IdentifierExpression("z"),
                    BinaryOperatorType.Subtract,
                    new ParenthesizedExpression(this.Z.Clone())
                ))));
            }
        }

        public RangedLayer(RuntimeLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");
            InitializeFromRuntime(this, layer);
        }

        private RangedLayer()
        {
        }

        public override string ToString()
        {
            return string.Format("[RangedLayer: X={0}, Y={1}, Z={2}, Width={3}, Height={4}, Depth={5}, Layer={6}, Inputs={7}]", X, Y, Z, Width, Height, Depth, Layer, Inputs);
        }

        public string GetPrintableStructure()
        {
            var str = "";
            foreach (var input in this.Inputs)
                if (input != null)
                    str += input.GetPrintableStructure() + Environment.NewLine;
            str += this.ToString();
            return str;
        }

        // Just finding offsets, then use them to determine max width, start X location, etc.
        public static void FindMaximumOffsets(
            RangedLayer layer, 
            out Expression OffsetX,
            out Expression OffsetY,
            out Expression OffsetZ)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            OffsetX = layer.OffsetX;
            OffsetY = layer.OffsetY;
            OffsetZ = layer.OffsetZ;

            foreach (var input in layer.Inputs)
            {
                if (input == null)
                    continue;
                
                Expression iOffsetX, iOffsetY, iOffsetZ;
                FindMaximumOffsets(input, out iOffsetX, out iOffsetY, out iOffsetZ);

                OffsetX = GetSmallestOrLargestExpression(OffsetX, iOffsetX, true);
                OffsetY = GetSmallestOrLargestExpression(OffsetY, iOffsetY, true);
                OffsetZ = GetSmallestOrLargestExpression(OffsetZ, iOffsetZ, true);
            }
        }

        /// <summary>
        /// Given the specified ranged layer, find expressions which determine the total
        /// extent of the main loop for the compilation process.
        /// </summary>
        public static void FindMaximumBounds(
            RangedLayer layer,
            out Expression x,
            out Expression y,
            out Expression z,
            out Expression width,
            out Expression height,
            out Expression depth,
            out Expression outerx,
            out Expression outery,
            out Expression outerz)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            // Set initial values.
            x = layer.X;
            y = layer.Y;
            z = layer.Z;
            width = layer.Width;
            height = layer.Height;
            depth = layer.Depth;
            outerx = layer.OuterX;
            outery = layer.OuterY;
            outerz = layer.OuterZ;

            // For each of the inputs, evaluate which is the appropriate
            // expression.
            foreach (var input in layer.Inputs)
            {
                if (input == null)
                    continue;

                Expression ix, iy, iz, iwidth, iheight, idepth, iouterx, ioutery, iouterz;
                FindMaximumBounds(input, out ix, out iy, out iz, out iwidth, out iheight, out idepth, out iouterx, out ioutery, out iouterz);

                x = GetSmallestOrLargestExpression(x, ix, false);
                y = GetSmallestOrLargestExpression(y, iy, false);
                z = GetSmallestOrLargestExpression(z, iz, false);
                width = GetSmallestOrLargestExpression(width, iwidth, true);
                height = GetSmallestOrLargestExpression(height, iheight, true);
                depth = GetSmallestOrLargestExpression(depth, idepth, true);
                outerx = GetSmallestOrLargestExpression(outerx, iouterx, true);
                outery = GetSmallestOrLargestExpression(outery, ioutery, true);
                outerz = GetSmallestOrLargestExpression(outerz, iouterz, true);
            }
        }

        /// <summary>
        /// Returns one expression based on whether to return the smallest
        /// or largest logical expression.
        /// </summary>
        private static Expression GetSmallestOrLargestExpression(Expression a, Expression b, bool largest)
        {
            // Use the visitor to replace the identifiers with numeric values.
            var aEvaluated = EvaluateExpression(a);
            var bEvaluated = EvaluateExpression(b);
            var aValue = (int)(aEvaluated as PrimitiveExpression).Value;
            var bValue = (int)(bEvaluated as PrimitiveExpression).Value;
            if ((!largest && aValue < bValue) ||
                (largest && aValue > bValue))
                return a;
            else
                return b;
        }

        private static int SanitizeValue(object value)
        {
            try
            {
                return (int)value;
            }
            catch
            {
            }
            try
            {
                return (int)(long)value;
            }
            catch
            {
            }
            throw new InvalidOperationException(value.GetType().FullName.ToString());
        }

        /// <summary>
        /// Evaluates the given expression down to just a PrimitiveExpression.
        /// </summary>
        public static PrimitiveExpression EvaluateExpression(Expression expr, Dictionary<string, object> values = null)
        {
            if (expr is BinaryOperatorExpression)
            {
                var a = EvaluateExpression((expr as BinaryOperatorExpression).Left, values);
                var b = EvaluateExpression((expr as BinaryOperatorExpression).Right, values);
                var op = (expr as BinaryOperatorExpression).Operator;
                int aValue = SanitizeValue(a.Value);
                int bValue = SanitizeValue(b.Value);
                switch (op)
                {
                    case BinaryOperatorType.Add:
                        return new PrimitiveExpression(SanitizeValue(aValue + bValue));
                    case BinaryOperatorType.Subtract:
                        return new PrimitiveExpression(SanitizeValue(aValue - bValue));
                    case BinaryOperatorType.Divide:
                        return new PrimitiveExpression(SanitizeValue(aValue / bValue));
                    case BinaryOperatorType.Multiply:
                        return new PrimitiveExpression(SanitizeValue(aValue * bValue));
                    default:
                        throw new NotSupportedException(op.ToString());
                }
            }
            else if (expr is ParenthesizedExpression)
                return EvaluateExpression((expr as ParenthesizedExpression).Expression, values);
            else if (expr is IdentifierExpression)
            {
                if (values != null)
                    return new PrimitiveExpression(SanitizeValue(values[(expr as IdentifierExpression).Identifier]));
                else
                    return new PrimitiveExpression(SanitizeValue(100));
            }
            else if (expr is PrimitiveExpression)
                return expr as PrimitiveExpression;
            else
                throw new NotSupportedException(expr.GetType().FullName);
        }

        /// <summary>
        /// Initializes the ranged layer from a runtime layer.
        /// </summary>
        private static void InitializeFromRuntime(RangedLayer ranged, RuntimeLayer layer)
        {
            // Set up the initial expressions.
            ranged.X = new IdentifierExpression("x");
            ranged.Y = new IdentifierExpression("y");
            ranged.Z = new IdentifierExpression("z");
            ranged.Width = new IdentifierExpression("width");
            ranged.Height = new IdentifierExpression("height");
            ranged.Depth = new IdentifierExpression("depth");

            // Go through all of the inputs and process them.
            var inputs = layer.GetInputs();
            ranged.Inputs = new RangedLayer[inputs.Length];
            for (var i = 0; i < inputs.Length; i++)
            {
                ranged.Inputs[i] = inputs[i] == null ? null : ProcessInput(ranged, layer, inputs[i]);
            }
        }

        /// <summary>
        /// Process an input runtime layer.
        /// </summary>
        /// <param name="ranged">The current ranged layer.</param>
        /// <param name="layer">The current runtime layer.</param>
        /// <param name="input">The input runtime layer.</param>
        /// <returns>The input ranged layer.</returns>
        private static RangedLayer ProcessInput(RangedLayer currentRanged, RuntimeLayer currentRuntime, RuntimeLayer inputRuntime)
        {
            // Set up the new ranged layer.
            var inputRanged = new RangedLayer();
            inputRanged.X = currentRanged.X;
            inputRanged.Y = currentRanged.Y;
            inputRanged.Z = currentRanged.Z;
            inputRanged.Width = currentRanged.Width;
            inputRanged.Height = currentRanged.Height;
            inputRanged.Depth = currentRanged.Depth;
            inputRanged.Layer = inputRuntime;

            // Determine if we need to adjust the width, height and depth.
            if (currentRuntime.Algorithm.InputWidthAtHalfSize)
                inputRanged.Width = CreateDivideByTwo(inputRanged.Width);
            if (currentRuntime.Algorithm.InputHeightAtHalfSize)
                inputRanged.Height = CreateDivideByTwo(inputRanged.Height);
            if (currentRuntime.Algorithm.InputDepthAtHalfSize)
                inputRanged.Depth = CreateDivideByTwo(inputRanged.Depth);

            // Determine if we need to adjust the X, Y or Z offsets.
            if (currentRuntime.Algorithm.RequiredXBorder > 0)
            {
                inputRanged.X = CreateSubtraction(inputRanged.X, currentRuntime.Algorithm.RequiredXBorder);
                //inputRanged.Width = CreateAddition(inputRanged.Width, currentRuntime.Algorithm.RequiredXBorder * 2);
            }
            if (currentRuntime.Algorithm.RequiredYBorder > 0)
            {
                inputRanged.Y = CreateSubtraction(inputRanged.Y, currentRuntime.Algorithm.RequiredYBorder);
                //inputRanged.Height = CreateAddition(inputRanged.Height, currentRuntime.Algorithm.RequiredYBorder * 2);
            }
            if (currentRuntime.Algorithm.RequiredZBorder > 0)
            {
                inputRanged.Z = CreateSubtraction(inputRanged.Z, currentRuntime.Algorithm.RequiredZBorder);
                //inputRanged.Depth = CreateAddition(inputRanged.Depth, currentRuntime.Algorithm.RequiredZBorder * 2);
            }

            // Process inputs recursively.
            var inputs = inputRuntime.GetInputs();
            inputRanged.Inputs = new RangedLayer[inputs.Length];
            for (var i = 0; i < inputs.Length; i++)
            {
                inputRanged.Inputs[i] = inputs[i] == null ? null : ProcessInput(inputRanged, inputRuntime, inputs[i]);
            }

            // Return the new layer.
            return inputRanged;
        }

        /// <summary>
        /// Creates a parethesized subtraction expression.
        /// </summary>
        private static Expression CreateSubtraction(Expression input, int border)
        {
            return new ParenthesizedExpression(
                new BinaryOperatorExpression(
                    input,
                    BinaryOperatorType.Subtract,
                    new PrimitiveExpression(border)
            )
            );
        }

        /// <summary>
        /// Creates a parethesized addition expression.
        /// </summary>
        private static Expression CreateAddition(Expression input, int border)
        {
            return new ParenthesizedExpression(
                new BinaryOperatorExpression(
                input,
                BinaryOperatorType.Add,
                new PrimitiveExpression(border)
            )
            );
        }
        
        /// <summary>
        /// Creates a parethesized divide-by-2 expression.
        /// </summary>
        private static Expression CreateDivideByTwo(Expression input)
        {
            return new ParenthesizedExpression(
                new BinaryOperatorExpression(
                input,
                BinaryOperatorType.Divide,
                new PrimitiveExpression(2)
            )
            );
        }
    }
}

