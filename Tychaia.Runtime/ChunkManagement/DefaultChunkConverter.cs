// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Data;

namespace Tychaia.Runtime
{
    public class DefaultChunkConverter : IChunkConverter
    {
        public void FromChunk<T>(Chunk chunk, T target) where T : IChunk
        {
            target.Cells = chunk.Cells;
            target.GeneratedIndices = chunk.Indexes;
            target.Generated = chunk.Generated;
            if (chunk.Vertexes == null)
            {
                target.GeneratedVertexes = new VertexPositionTexture[0];
            }
            else
            {
                target.GeneratedVertexes =
                    chunk.Vertexes.Select(
                        x => new VertexPositionTexture(new Vector3(x.X, x.Y, x.Z), new Vector2(x.U, x.V))).ToArray();
            }

            if (target.IndexBuffer != null)
            {
                target.IndexBuffer.Dispose();
                target.IndexBuffer = null;
            }

            if (target.VertexBuffer != null)
            {
                target.VertexBuffer.Dispose();
                target.VertexBuffer = null;
            }
        }

        public Chunk ToChunk<T>(T chunk) where T : IChunk
        {
            var vertexes = new Vertex[0];
            if (chunk.GeneratedVertexes != null)
            {
                vertexes = chunk.GeneratedVertexes.Select(
                    x =>
                    new Vertex
                    {
                        U = x.TextureCoordinate.X,
                        V = x.TextureCoordinate.Y,
                        X = x.Position.X,
                        Y = x.Position.Y,
                        Z = x.Position.Z
                    }).ToArray();
            }

            return new Chunk
            {
                Cells = chunk.Cells, 
                Indexes = chunk.GeneratedIndices, 
                Vertexes = vertexes, 
                Generated = chunk.Generated,
                X = chunk.X, 
                Y = chunk.Y, 
                Z = chunk.Z
            };
        }
    }
}