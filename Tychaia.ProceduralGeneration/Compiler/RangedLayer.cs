// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using Tychaia.ProceduralGeneration.AstVisitors;

namespace Tychaia.ProceduralGeneration.Compiler
{
    /// <summary>
    /// Contains the start and end loop values at and the layer which is activated
    /// when they become true.
    /// </summary>
    public class RangedLayer
    {
        public RangedLayer(RuntimeLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");
            InitializeFromRuntime(this, layer);
        }

        private RangedLayer()
        {
        }

        public Expression X { get; set; }
        public Expression Y { get; set; }
        public Expression Z { get; set; }
        public Expression Width { get; set; }
        public Expression Height { get; set; }
        public Expression Depth { get; set; }
        public Expression OuterX { get; set; }
        public Expression OuterY { get; set; }
        public Expression OuterZ { get; set; }
        public Expression CalculationStartI { get; set; }
        public Expression CalculationStartJ { get; set; }
        public Expression CalculationStartK { get; set; }
        public Expression CalculationEndI { get; set; }
        public Expression CalculationEndJ { get; set; }
        public Expression CalculationEndK { get; set; }
        public Expression OffsetI { get; set; }
        public Expression OffsetJ { get; set; }
        public Expression OffsetK { get; set; }
        public Expression OffsetX { get; set; }
        public Expression OffsetY { get; set; }
        public Expression OffsetZ { get; set; }
        public RuntimeLayer Layer { get; set; }
        public RangedLayer[] Inputs { get; set; }

        public override string ToString()
        {
            return string.Format(
                "[RangedLayer: " +
                "X={0}, " +
                "Y={1}, " +
                "Z={2}, " +
                "Width={3}, " +
                "Height={4}, " +
                "Depth={5}, " +
                "OuterX={6}, " +
                "OuterY={7}, " +
                "OuterZ={8}, " +
                "Layer={9}, " +
                "Inputs={10}" +
                "]",
                this.X,
                this.Y,
                this.Z,
                this.Width,
                this.Height,
                this.Depth,
                this.OuterX,
                this.OuterY,
                this.OuterZ,
                this.Layer,
                this.Inputs);
        }

        public string GetPrintableStructure()
        {
            var str = string.Empty;
            foreach (var input in this.Inputs)
                if (input != null)
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
                FindMaximumBounds(
                    input, 
                    out ix, 
                    out iy, 
                    out iz, 
                    out iwidth, 
                    out iheight, 
                    out idepth, 
                    out iouterx,
                    out ioutery, 
                    out iouterz);

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

            throw new InvalidOperationException(value.GetType().FullName);
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

            if (expr is ParenthesizedExpression)
                return EvaluateExpression((expr as ParenthesizedExpression).Expression, values);
            if (expr is IdentifierExpression)
            {
                if (values != null)
                    return new PrimitiveExpression(SanitizeValue(values[(expr as IdentifierExpression).Identifier]));
                return new PrimitiveExpression(SanitizeValue(100));
            }

            if (expr is UnaryOperatorExpression &&
                (expr as UnaryOperatorExpression).Operator == UnaryOperatorType.Minus)
            {
                var primitive = EvaluateExpression((expr as UnaryOperatorExpression).Expression);
                return new PrimitiveExpression(-((dynamic)primitive.Value));
            }

            if (expr is PrimitiveExpression)
                return expr as PrimitiveExpression;
            throw new NotSupportedException(expr.GetType().FullName);
        }

        /// <summary>
        /// Applies adjustments to an expression when calculating the start position for the root layer.
        /// </summary>
        private static PrimitiveExpression DetermineCalculationStartForRootLayerFromMaximumBound(
            Expression bound,
            string xyzName)
        {
            return EvaluateExpression(
                new UnaryOperatorExpression(
                    UnaryOperatorType.Minus,
                    EvaluateExpression(
                        new BinaryOperatorExpression(
                            new PrimitiveExpression(2),
                            BinaryOperatorType.Multiply,
                            EvaluateExpression(
                                new BinaryOperatorExpression(
                                    bound.Clone(),
                                    BinaryOperatorType.Subtract,
                                    new IdentifierExpression(xyzName)),
                                new Dictionary<string, object> { { xyzName, 0 } })))));
        }

        /// <summary>
        /// Applies adjustments to an expression when calculating the absolute offset for the root layer.
        /// </summary>
        private static PrimitiveExpression DetermineAbsoluteOffsetForRootLayerFromMaximumBound(
            Expression bound,
            string xyzName)
        {
            return EvaluateExpression(
                new BinaryOperatorExpression(
                    bound.Clone(),
                    BinaryOperatorType.Subtract,
                    new IdentifierExpression(xyzName)),
                new Dictionary<string, object> { { xyzName, 0 } });
        }

        /// <summary>
        /// Applies adjustments to an expression when calculating the relative offset for a layer.
        /// </summary>
        private static PrimitiveExpression DetermineRelativeOffsetForLayerFromXYZ(
            Expression startIJK,
            Expression xyz,
            Expression minXYZ,
            string xyzName)
        {
            return EvaluateExpression(
                new UnaryOperatorExpression(
                    UnaryOperatorType.Minus,
                    new BinaryOperatorExpression(
                        startIJK.Clone(),
                        BinaryOperatorType.Subtract,
                        new BinaryOperatorExpression(
                            xyz.Clone(),
                            BinaryOperatorType.Subtract,
                            minXYZ.Clone()))),
                new Dictionary<string, object> { { xyzName, 0 } });
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
            ranged.Layer = layer;
            ranged.Width = new IdentifierExpression("width");
            ranged.Height = new IdentifierExpression("height");
            ranged.Depth = new IdentifierExpression("depth");
            ranged.OuterX = new BinaryOperatorExpression(
                ranged.X.Clone(),
                BinaryOperatorType.Add,
                ranged.Width.Clone());
            ranged.OuterY = new BinaryOperatorExpression(
                ranged.Y.Clone(),
                BinaryOperatorType.Add,
                ranged.Height.Clone());
            ranged.OuterZ = new BinaryOperatorExpression(
                ranged.Z.Clone(),
                BinaryOperatorType.Add,
                ranged.Depth.Clone());

            // Go through all of the inputs and process them.
            var inputs = layer.GetInputs();
            ranged.Inputs = new RangedLayer[inputs.Length];
            for (var i = 0; i < inputs.Length; i++)
            {
                ranged.Inputs[i] = inputs[i] == null ? null : ProcessInput(ranged, layer, inputs[i], i);
            }

            // Now we have to find the maximum values.
            Expression mx, my, mz, mwidth, mheight, mdepth, mouterx, moutery, mouterz;
            FindMaximumBounds(
                ranged,
                out mx, 
                out my, 
                out mz,
                out mwidth, 
                out mheight, 
                out mdepth,
                out mouterx, 
                out moutery, 
                out mouterz);

            // And recalculate back all of the calculation start / end values.
            ranged.CalculationStartI = DetermineCalculationStartForRootLayerFromMaximumBound(mx.Clone(), "x");
            ranged.CalculationStartJ = DetermineCalculationStartForRootLayerFromMaximumBound(my.Clone(), "y");
            ranged.CalculationStartK = DetermineCalculationStartForRootLayerFromMaximumBound(mz.Clone(), "z");
            ranged.CalculationEndI = mwidth.Clone();
            ranged.CalculationEndJ = mheight.Clone();
            ranged.CalculationEndK = mdepth.Clone();
            ranged.OffsetI = DetermineRelativeOffsetForLayerFromXYZ(ranged.CalculationStartI, ranged.X, mx, "x");
            ranged.OffsetJ = DetermineRelativeOffsetForLayerFromXYZ(ranged.CalculationStartJ, ranged.Y, my, "y");
            ranged.OffsetK = DetermineRelativeOffsetForLayerFromXYZ(ranged.CalculationStartK, ranged.Z, mz, "z");
            ranged.OffsetX = DetermineAbsoluteOffsetForRootLayerFromMaximumBound(mx.Clone(), "x");
            ranged.OffsetY = DetermineAbsoluteOffsetForRootLayerFromMaximumBound(my.Clone(), "y");
            ranged.OffsetZ = DetermineAbsoluteOffsetForRootLayerFromMaximumBound(mz.Clone(), "z");

            // Go through all of the inputs and backfill them.
            for (var i = 0; i < inputs.Length; i++)
            {
                BackfillInput(ranged, layer, ranged.Inputs[i], i, mx, my, mz);
            }
        }

        /// <summary>
        /// Backfills the ranged input layer to calculate the CalculationStart/End properties.
        /// </summary>
        /// <param name="currentRanged">The current ranged layer.</param>
        /// <param name="currentRuntime">The current runtime layer.</param>
        /// <param name="inputRanged">The input ranged layer.</param>
        /// <param name="idx">The index of the input layer in the parent layer.</param>
        private static void BackfillInput(
            RangedLayer currentRanged,
            RuntimeLayer currentRuntime,
            RangedLayer inputRanged,
            int idx,
            Expression mx,
            Expression my,
            Expression mz)
        {
            inputRanged.CalculationStartI = currentRanged.CalculationStartI.Clone();
            inputRanged.CalculationStartJ = currentRanged.CalculationStartJ.Clone();
            inputRanged.CalculationStartK = currentRanged.CalculationStartK.Clone();
            inputRanged.CalculationEndI = currentRanged.CalculationEndI.Clone();
            inputRanged.CalculationEndJ = currentRanged.CalculationEndJ.Clone();
            inputRanged.CalculationEndK = currentRanged.CalculationEndK.Clone();
            inputRanged.OffsetX = currentRanged.OffsetX.Clone();
            inputRanged.OffsetY = currentRanged.OffsetY.Clone();
            inputRanged.OffsetZ = currentRanged.OffsetZ.Clone();

            if (currentRuntime.Algorithm.RequiredXBorder[idx] > 0)
                inputRanged.CalculationStartI = CreateSubtraction(
                    inputRanged.CalculationStartI,
                    currentRuntime.Algorithm.RequiredXBorder[idx] * 2);
            if (currentRuntime.Algorithm.RequiredYBorder[idx] > 0)
                inputRanged.CalculationStartJ = CreateSubtraction(
                    inputRanged.CalculationStartJ,
                    currentRuntime.Algorithm.RequiredYBorder[idx] * 2);
            if (currentRuntime.Algorithm.RequiredZBorder[idx] > 0)
                inputRanged.CalculationStartK = CreateSubtraction(
                    inputRanged.CalculationStartK,
                    currentRuntime.Algorithm.RequiredZBorder[idx] * 2);

            if (currentRuntime.Algorithm.InputWidthAtHalfSize[idx])
                inputRanged.CalculationEndI = CreateDivideByTwo(inputRanged.CalculationEndI);
            if (currentRuntime.Algorithm.InputHeightAtHalfSize[idx])
                inputRanged.CalculationEndJ = CreateDivideByTwo(inputRanged.CalculationEndJ);
            if (currentRuntime.Algorithm.InputDepthAtHalfSize[idx])
                inputRanged.CalculationEndK = CreateDivideByTwo(inputRanged.CalculationEndK);

            inputRanged.OffsetI = DetermineRelativeOffsetForLayerFromXYZ(
                inputRanged.CalculationStartI,
                inputRanged.X,
                mx,
                "x");
            inputRanged.OffsetJ = DetermineRelativeOffsetForLayerFromXYZ(
                inputRanged.CalculationStartJ,
                inputRanged.Y,
                my,
                "y");
            inputRanged.OffsetK = DetermineRelativeOffsetForLayerFromXYZ(
                inputRanged.CalculationStartK,
                inputRanged.Z,
                mz,
                "z");

            // Run AST visitors over the expression to simplify.
            inputRanged.CalculationStartI = AstHelpers.OptimizeExpression(inputRanged.CalculationStartI);
            inputRanged.CalculationStartJ = AstHelpers.OptimizeExpression(inputRanged.CalculationStartJ);
            inputRanged.CalculationStartK = AstHelpers.OptimizeExpression(inputRanged.CalculationStartK);
            inputRanged.CalculationEndI = AstHelpers.OptimizeExpression(inputRanged.CalculationEndI);
            inputRanged.CalculationEndJ = AstHelpers.OptimizeExpression(inputRanged.CalculationEndJ);
            inputRanged.CalculationEndK = AstHelpers.OptimizeExpression(inputRanged.CalculationEndK);
            inputRanged.OffsetI = AstHelpers.OptimizeExpression(inputRanged.OffsetI);
            inputRanged.OffsetJ = AstHelpers.OptimizeExpression(inputRanged.OffsetJ);
            inputRanged.OffsetK = AstHelpers.OptimizeExpression(inputRanged.OffsetK);
            inputRanged.OffsetX = AstHelpers.OptimizeExpression(inputRanged.OffsetX);
            inputRanged.OffsetY = AstHelpers.OptimizeExpression(inputRanged.OffsetY);
            inputRanged.OffsetZ = AstHelpers.OptimizeExpression(inputRanged.OffsetZ);

            // Go through all of the inputs and backfill them.
            for (var i = 0; i < inputRanged.Inputs.Length; i++)
            {
                BackfillInput(inputRanged, inputRanged.Layer, inputRanged.Inputs[i], i, mx, my, mz);
            }
        }

        /// <summary>
        /// Process an input runtime layer.
        /// </summary>
        /// <param name="ranged">The current ranged layer.</param>
        /// <param name="layer">The current runtime layer.</param>
        /// <param name="input">The input runtime layer.</param>
        /// <param name="idx">The index of the input layer in the parent layer.</param>
        /// <returns>The input ranged layer.</returns>
        private static RangedLayer ProcessInput(
            RangedLayer currentRanged,
            RuntimeLayer currentRuntime,
            RuntimeLayer inputRuntime,
            int idx)
        {
            // Set up the new ranged layer.
            var inputRanged = new RangedLayer();
            inputRanged.X = currentRanged.X;
            inputRanged.Y = currentRanged.Y;
            inputRanged.Z = currentRanged.Z;
            inputRanged.Width = currentRanged.Width;
            inputRanged.Height = currentRanged.Height;
            inputRanged.Depth = currentRanged.Depth;
            inputRanged.OuterX = currentRanged.OuterX;
            inputRanged.OuterY = currentRanged.OuterY;
            inputRanged.OuterZ = currentRanged.OuterZ;
            inputRanged.Layer = inputRuntime;

            // Determine if we need to adjust the width, height and depth.
            if (currentRuntime.Algorithm.InputWidthAtHalfSize[idx])
            {
                inputRanged.Width = CreateDivideByTwo(inputRanged.Width);
                inputRanged.OuterX = CreateDivideByTwo(inputRanged.OuterX);
            }

            if (currentRuntime.Algorithm.InputHeightAtHalfSize[idx])
            {
                inputRanged.Height = CreateDivideByTwo(inputRanged.Height);
                inputRanged.OuterY = CreateDivideByTwo(inputRanged.OuterY);
            }

            if (currentRuntime.Algorithm.InputDepthAtHalfSize[idx])
            {
                inputRanged.Depth = CreateDivideByTwo(inputRanged.Depth);
                inputRanged.OuterZ = CreateDivideByTwo(inputRanged.OuterZ);
            }

            // Determine if we need to adjust the X, Y or Z offsets.
            if (currentRuntime.Algorithm.RequiredXBorder[idx] > 0)
            {
                inputRanged.X = CreateSubtraction(
                    inputRanged.X,
                    currentRuntime.Algorithm.RequiredXBorder[idx]);
                inputRanged.Width = CreateAddition(
                    inputRanged.Width,
                    currentRuntime.Algorithm.RequiredXBorder[idx] * 2);
                inputRanged.OuterX = CreateAddition(
                    inputRanged.OuterX,
                    currentRuntime.Algorithm.RequiredXBorder[idx]);
            }

            if (currentRuntime.Algorithm.RequiredYBorder[idx] > 0)
            {
                inputRanged.Y = CreateSubtraction(
                    inputRanged.Y,
                    currentRuntime.Algorithm.RequiredYBorder[idx]);
                inputRanged.Height = CreateAddition(
                    inputRanged.Height,
                    currentRuntime.Algorithm.RequiredYBorder[idx] * 2);
                inputRanged.OuterY = CreateAddition(
                    inputRanged.OuterY,
                    currentRuntime.Algorithm.RequiredYBorder[idx]);
            }

            if (currentRuntime.Algorithm.RequiredZBorder[idx] > 0)
            {
                inputRanged.Z = CreateSubtraction(
                    inputRanged.Z,
                    currentRuntime.Algorithm.RequiredZBorder[idx]);
                inputRanged.Depth = CreateAddition(
                    inputRanged.Depth,
                    currentRuntime.Algorithm.RequiredZBorder[idx] * 2);
                inputRanged.OuterZ = CreateAddition(
                    inputRanged.OuterZ,
                    currentRuntime.Algorithm.RequiredZBorder[idx]);
            }

            // Process inputs recursively.
            var inputs = inputRuntime.GetInputs();
            inputRanged.Inputs = new RangedLayer[inputs.Length];
            for (var i = 0; i < inputs.Length; i++)
            {
                inputRanged.Inputs[i] =
                    inputs[i] == null
                        ? null
                        : ProcessInput(inputRanged, inputRuntime, inputs[i], i);
            }

            // Run AST visitors over the expression to simplify.
            inputRanged.X = AstHelpers.OptimizeExpression(inputRanged.X);
            inputRanged.Y = AstHelpers.OptimizeExpression(inputRanged.Y);
            inputRanged.Z = AstHelpers.OptimizeExpression(inputRanged.Z);
            inputRanged.Width = AstHelpers.OptimizeExpression(inputRanged.Width);
            inputRanged.Height = AstHelpers.OptimizeExpression(inputRanged.Height);
            inputRanged.Depth = AstHelpers.OptimizeExpression(inputRanged.Depth);
            inputRanged.OuterX = AstHelpers.OptimizeExpression(inputRanged.OuterX);
            inputRanged.OuterY = AstHelpers.OptimizeExpression(inputRanged.OuterY);
            inputRanged.OuterZ = AstHelpers.OptimizeExpression(inputRanged.OuterZ);

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
                    input.Clone(),
                    BinaryOperatorType.Subtract,
                    new PrimitiveExpression(border)));
        }

        /// <summary>
        /// Creates a parethesized addition expression.
        /// </summary>
        private static Expression CreateAddition(Expression input, int border)
        {
            return new ParenthesizedExpression(
                new BinaryOperatorExpression(
                    input.Clone(),
                    BinaryOperatorType.Add,
                    new PrimitiveExpression(border)));
        }

        /// <summary>
        /// Creates a parethesized divide-by-2 expression.
        /// </summary>
        private static Expression CreateDivideByTwo(Expression input)
        {
            return new ParenthesizedExpression(
                new BinaryOperatorExpression(
                    input.Clone(),
                    BinaryOperatorType.Divide,
                    new PrimitiveExpression(2)));
        }
    }
}
