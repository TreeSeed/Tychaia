// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Tychaia
{
    public class DefaultDebugCubeRenderer : IDebugCubeRenderer
    {
        public void RenderWireframeCube(
            IRenderContext renderContext,
            Microsoft.Xna.Framework.BoundingBox boundingBox,
            Color? color = null)
        {
            this.RenderWireframeCube(renderContext, boundingBox.ToProtogame(), color);
        }

        public void RenderWireframeCube(
            IRenderContext renderContext,
            IBoundingBox boundingBox,
            Color? color = null)
        {
            if (color == null) color = Color.White;
            var vertexes = new[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), color.Value),
                new VertexPositionColor(new Vector3(0, 0, 1), color.Value),
                new VertexPositionColor(new Vector3(0, 1, 0), color.Value),
                new VertexPositionColor(new Vector3(0, 1, 1), color.Value),
                new VertexPositionColor(new Vector3(1, 0, 0), color.Value),
                new VertexPositionColor(new Vector3(1, 0, 1), color.Value),
                new VertexPositionColor(new Vector3(1, 1, 0), color.Value),
                new VertexPositionColor(new Vector3(1, 1, 1), color.Value)
            };

            var indicies = new short[]
            {
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
            };

            renderContext.EnableVertexColors();
            var world = renderContext.World;
            var xnaBoundingBox = boundingBox.ToXna();
            renderContext.World =
                Matrix.CreateScale(xnaBoundingBox.Max - xnaBoundingBox.Min) *
                Matrix.CreateTranslation(xnaBoundingBox.Min);

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertexes,
                    0,
                    vertexes.Length,
                    indicies,
                    0,
                    vertexes.Length / 2);
            }
            renderContext.World = world;
        }
    }
}

