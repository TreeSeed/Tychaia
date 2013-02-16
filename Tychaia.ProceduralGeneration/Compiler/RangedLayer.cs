//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using ICSharpCode.NRefactory.CSharp;

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

        public RangedLayer(RuntimeLayer layer)
        {
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
                str += input.GetPrintableStructure() + Environment.NewLine;
            str += this.ToString();
            return str;
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
            out Expression depth)
        {
            // Set initial values.
            x = layer.X;
            y = layer.Y;
            z = layer.Z;
            width = layer.Width;
            height = layer.Height;
            depth = layer.Depth;

            // For each of the inputs, evaluate which is the appropriate
            // expression.
            foreach (var input in layer.Inputs)
            {
                Expression ix, iy, iz, iwidth, iheight, idepth;
                FindMaximumBounds(input, out ix, out iy, out iz, out iwidth, out iheight, out idepth);

                x = GetSmallestOrLargestExpression(x, ix, false);
                y = GetSmallestOrLargestExpression(y, iy, false);
                z = GetSmallestOrLargestExpression(z, iz, false);
                width = GetSmallestOrLargestExpression(width, iwidth, true);
                height = GetSmallestOrLargestExpression(height, iheight, true);
                depth = GetSmallestOrLargestExpression(depth, idepth, true);
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

        /// <summary>
        /// Evaluates the given expression down to just a PrimitiveExpression.
        /// </summary>
        private static PrimitiveExpression EvaluateExpression(Expression expr)
        {
            if (expr is BinaryOperatorExpression)
            {
                var a = EvaluateExpression((expr as BinaryOperatorExpression).Left);
                var b = EvaluateExpression((expr as BinaryOperatorExpression).Right);
                var op = (expr as BinaryOperatorExpression).Operator;
                switch (op)
                {
                    case BinaryOperatorType.Add:
                        return new PrimitiveExpression((int)a.Value + (int)b.Value);
                    case BinaryOperatorType.Subtract:
                        return new PrimitiveExpression((int)a.Value - (int)b.Value);
                    case BinaryOperatorType.Divide:
                        return new PrimitiveExpression((int)a.Value / (int)b.Value);
                    case BinaryOperatorType.Multiply:
                        return new PrimitiveExpression((int)a.Value * (int)b.Value);
                    default:
                        throw new NotSupportedException(op.ToString());
                }
            }
            else if (expr is ParenthesizedExpression)
                return EvaluateExpression((expr as ParenthesizedExpression).Expression);
            else if (expr is IdentifierExpression)
                return new PrimitiveExpression(100);
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
                ranged.Inputs[i] = ProcessInput(ranged, layer, inputs[i]);
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
                inputRanged.Width = CreateAddition(inputRanged.Width, currentRuntime.Algorithm.RequiredXBorder);
            }
            if (currentRuntime.Algorithm.RequiredYBorder > 0)
            {
                inputRanged.Y = CreateSubtraction(inputRanged.Y, currentRuntime.Algorithm.RequiredYBorder);
                inputRanged.Height = CreateAddition(inputRanged.Height, currentRuntime.Algorithm.RequiredYBorder);
            }
            if (currentRuntime.Algorithm.RequiredZBorder > 0)
            {
                inputRanged.Z = CreateSubtraction(inputRanged.Z, currentRuntime.Algorithm.RequiredZBorder);
                inputRanged.Depth = CreateAddition(inputRanged.Depth, currentRuntime.Algorithm.RequiredZBorder);
            }

            // Process inputs recursively.
            var inputs = inputRuntime.GetInputs();
            inputRanged.Inputs = new RangedLayer[inputs.Length];
            for (var i = 0; i < inputs.Length; i++)
            {
                inputRanged.Inputs[i] = ProcessInput(inputRanged, inputRuntime, inputs[i]);
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

