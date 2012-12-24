using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame.Efficiency
{
	/// <remarks>
	/// Sourced from http://social.msdn.microsoft.com/forums/en-US/xnaframework/thread/7e5cc12f-3b75-49ef-8da9-c00a75139a21/.
	/// </remarks>
	internal class DepthSpriteBatch
	{
		private VertexPositionColorTexture[] vertices;
		private short[] indices;
		private int vertexCount = 0;
		private int indexCount = 0;
		private Texture2D texture;
		private VertexDeclaration declaration;
		private GraphicsDevice device;

		//  these should really be properties
		public Matrix World {
			get;
			set;
		}

		public Matrix View {
			get;
			set;
		}

		public Matrix Projection {
			get;
			set;
		}

		public Effect Effect {
			get;
			set;
		}

		public DepthSpriteBatch (GraphicsDevice device)
		{
			this.device = device;
			this.vertices = new VertexPositionColorTexture[256];
			this.indices = new short[vertices.Length * 3 / 2];
		}

		public void ResetMatrices (int width, int height)
		{
			this.World = Matrix.Identity;
			this.View = new Matrix (
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
			this.Projection = Matrix.CreateOrthographicOffCenter (
                0, width, -height, 0, 0, 1);
		}

		public void Draw (Texture2D texture, Vector2 dst, Color color, float z)
		{
			this.Draw (texture, texture.Bounds, new Rectangle ((int)dst.X, (int)dst.Y, texture.Bounds.Width, texture.Bounds.Height), color, z);
		}

		public void Draw (Texture2D texture, Rectangle dst, Color color, float z)
		{
			this.Draw (texture, texture.Bounds, dst, color, z);
		}

		public void Draw (Texture2D texture, Rectangle srcRectangle, Rectangle dstRectangle, Color color, float z)
		{
			//  if the texture changes, we flush all queued sprites.
			if (this.texture != null && this.texture != texture)
				this.Flush ();
			this.texture = texture;

			//  ensure space for my vertices and indices.
			this.EnsureSpace (6, 4);

			//  add the new indices
			indices [indexCount++] = (short)(vertexCount + 0);
			indices [indexCount++] = (short)(vertexCount + 1);
			indices [indexCount++] = (short)(vertexCount + 3);
			indices [indexCount++] = (short)(vertexCount + 1);
			indices [indexCount++] = (short)(vertexCount + 2);
			indices [indexCount++] = (short)(vertexCount + 3);

			// add the new vertices
			vertices [vertexCount++] = new VertexPositionColorTexture (
                new Vector3 (dstRectangle.Left, dstRectangle.Top, z)
                , color, GetUV (srcRectangle.Left, srcRectangle.Top));
			vertices [vertexCount++] = new VertexPositionColorTexture (
                new Vector3 (dstRectangle.Right, dstRectangle.Top, z)
                , color, GetUV (srcRectangle.Right, srcRectangle.Top));
			vertices [vertexCount++] = new VertexPositionColorTexture (
                new Vector3 (dstRectangle.Right, dstRectangle.Bottom, z)
                , color, GetUV (srcRectangle.Right, srcRectangle.Bottom));
			vertices [vertexCount++] = new VertexPositionColorTexture (
                new Vector3 (dstRectangle.Left, dstRectangle.Bottom, z)
                , color, GetUV (srcRectangle.Left, srcRectangle.Bottom));

			//  we premultiply all vertices times the world matrix.
			//  the world matrix changes alot and we don't want to have to flush
			//  every time it changes.
			Matrix world = this.World;
			for (int i = vertexCount - 4; i < vertexCount; i++)
				Vector3.Transform (ref vertices [i].Position, ref world, out vertices [i].Position);
		}

		Vector2 GetUV (float x, float y)
		{
			return new Vector2 (x / (float)texture.Width, y / (float)texture.Height);
		}

		void EnsureSpace (int indexSpace, int vertexSpace)
		{
			if (indexCount + indexSpace >= indices.Length)
				Array.Resize (ref indices, Math.Max (indexCount + indexSpace, indices.Length * 2));
			if (vertexCount + vertexSpace >= vertices.Length)
				Array.Resize (ref vertices, Math.Max (vertexCount + vertexSpace, vertices.Length * 2));
		}

		public void Flush ()
		{
			if (this.vertexCount > 0) {
				Effect effect = this.Effect;
				//  set the only parameter this effect takes.
				effect.Parameters ["MatrixTransform"].SetValue (this.View * this.Projection);
				effect.Parameters ["Texture"].SetValue (this.texture);

				effect.CurrentTechnique.Passes [0].Apply ();

				device.BlendState = BlendState.AlphaBlend;
				device.DrawUserIndexedPrimitives<VertexPositionColorTexture> (
                    PrimitiveType.TriangleList, this.vertices, 0, this.vertexCount,
                    this.indices, 0, this.indexCount / 3);

				this.vertexCount = 0;
				this.indexCount = 0;
			}
		}

	}
}
