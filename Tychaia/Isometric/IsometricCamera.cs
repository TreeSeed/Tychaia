// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Tychaia
{
    public class IsometricCamera : IIsometricCamera
    {
        /// <summary>
        /// The X position in 3D space where we are focusing the camera.
        /// </summary>
        private long m_CurrentX;

        /// <summary>
        /// The Y position in 3D space where we are focusing the camera.
        /// </summary>
        private long m_CurrentY;

        /// <summary>
        /// The Z position in 3D space where we are focusing the camera.
        /// </summary>
        private long m_CurrentZ;

        public IsometricCamera(ChunkOctree octree, Chunk chunk)
        {
            if (octree == null) throw new ArgumentNullException("octree");
            if (chunk == null) throw new ArgumentNullException("chunk");
            this.ChunkOctree = octree;
            this.Chunk = chunk;
        }

        public Vector3 CurrentFocus
        {
            get { return new Vector3((float) this.m_CurrentX, (float) this.m_CurrentY, (float) this.m_CurrentZ); }
        }

        /// <summary>
        /// The X position on the screen of the current chunk.
        /// </summary>
        public int ChunkCenterX { get; set; }

        /// <summary>
        /// The Y position on the screen of the current chunk.
        /// </summary>
        public int ChunkCenterY { get; set; }

        /// <summary>
        /// The chunk that is currently the focus of the camera.
        /// </summary>
        public Chunk Chunk { get; private set; }

        /// <summary>
        /// The octree that holds all of the chunks.
        /// </summary>
        public ChunkOctree ChunkOctree { get; private set; }

        /// <summary>
        /// Checks the current relative position, applying the relative X, Y and Z positions
        /// to it and adjusting the center chunk as needed.
        /// </summary>
        public void Pan(long x, long y, long z)
        {
            this.Focus(
                this.m_CurrentX + x,
                this.m_CurrentY + y,
                this.m_CurrentZ + z);
        }

        /// <summary>
        /// Sets the current focus of the screen such the center of the screen
        /// is focused on the 3D point in the isometric world.
        /// </summary>
        public void Focus(long x, long y, long z)
        {
            // Skip if there is no octree.
            if (this.ChunkOctree == null)
                return;

            // Adjust the position.
            this.m_CurrentX = x;
            this.m_CurrentY = y;
            this.m_CurrentZ = z;

            // Pan current chunk.
            var newChunk = this.ChunkOctree.Get(
                this.m_CurrentX,
                this.m_CurrentY,
                this.m_CurrentZ);
            if (newChunk != null)
                this.Chunk = newChunk;
        }

        private int m_Rotation;
        public void InitializeRenderContext(IRenderContext renderContext)
        {
            //m_Rotation++;
            renderContext.View = Matrix.CreateLookAt(
                this.CurrentFocus + Vector3.Transform(new Vector3(15, 30, 15) * 35, Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation))),
                this.CurrentFocus,
                Vector3.Up);
            renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 5000.0f);
        }
    }
}