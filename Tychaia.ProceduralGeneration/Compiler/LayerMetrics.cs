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
    /// This calculates the expression trees that represent the offsets and
    /// sizes of layer calculations in a specific configuration, taking into
    /// account borders and half inputs.
    /// </summary>
    public static class LayerMetrics
    {
        /// <summary>
        /// Creates a new expression that adjusts the offset expression by the
        /// specified border amount.
        /// </summary>
        private static void AdjustOffsetExpression(ref Expression offset, int border)
        {
            // If the expression is already in the form "x - n" where n is a primitive
            // expression, we can adjust the existing expression instead of creating a
            // new one.
            if (offset is BinaryOperatorExpression &&
                (offset as BinaryOperatorExpression).Operator == BinaryOperatorType.Subtract &&
                (offset as BinaryOperatorExpression).Right is PrimitiveExpression &&
                ((offset as BinaryOperatorExpression).Right as PrimitiveExpression).Value is int)
            {
                // We have to use this form of assignment because we can't cast the left side
                // to a particular type to perform the relative addition.
                int value = (int)((offset as BinaryOperatorExpression).Right as PrimitiveExpression).Value;
                value += border;
                ((offset as BinaryOperatorExpression).Right as PrimitiveExpression).Value = value;
            }
            else
                offset = new BinaryOperatorExpression(offset, BinaryOperatorType.Subtract, new PrimitiveExpression(border));
        }

        /// <summary>
        /// Creates a new expression that adjusts the size expression by the
        /// specified border amount.
        /// </summary>
        private static void AdjustSizeExpression(ref Expression size, int border)
        {
            // If the expression is already in the form "x + n" where n is a primitive
            // expression, we can adjust the existing expression instead of creating a
            // new one.
            if (size is BinaryOperatorExpression &&
                (size as BinaryOperatorExpression).Operator == BinaryOperatorType.Add &&
                (size as BinaryOperatorExpression).Right is PrimitiveExpression &&
                ((size as BinaryOperatorExpression).Right as PrimitiveExpression).Value is int)
            {
                // We have to use this form of assignment because we can't cast the left side
                // to a particular type to perform the relative addition.
                int value = (int)((size as BinaryOperatorExpression).Right as PrimitiveExpression).Value;
                value += border * 2;
                ((size as BinaryOperatorExpression).Right as PrimitiveExpression).Value = value;
            }
            else
                size = new BinaryOperatorExpression(size, BinaryOperatorType.Add, new PrimitiveExpression(border * 2));
        }

        /// <summary>
        /// Determines the maximum size of the loop that will need to run to
        /// calculate all of the layer information.
        /// </summary>
        public static void DetermineMaximumLoopRequired(RuntimeLayer result,
                                                        ref Expression xOffset,
                                                        ref Expression yOffset,
                                                        ref Expression zOffset,
                                                        ref Expression width,
                                                        ref Expression height,
                                                        ref Expression depth)
        {
            // FIXME: This is not a perfect calculation.  Once the first half input
            // is hit, it stops.  This means that if you have a very large border on
            // one of the inputs below that point, which cancels out the half input,
            // the resulting code would crash.

            // Apply the current layer's border requirements.
            AdjustOffsetExpression(ref xOffset, result.Algorithm.RequiredXBorder);
            AdjustOffsetExpression(ref yOffset, result.Algorithm.RequiredYBorder);
            AdjustOffsetExpression(ref zOffset, result.Algorithm.RequiredZBorder);
            AdjustSizeExpression(ref width, result.Algorithm.RequiredXBorder);
            AdjustSizeExpression(ref height, result.Algorithm.RequiredYBorder);
            AdjustSizeExpression(ref depth, result.Algorithm.RequiredZBorder);

            // Add parent data.
            foreach (var p in result.GetInputs())
            {
                DetermineMaximumLoopRequired(p,
                                             ref xOffset,
                                             ref yOffset,
                                             ref zOffset,
                                             ref width,
                                             ref height,
                                             ref depth);
            }
        }
    }
}

