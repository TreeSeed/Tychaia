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
        private double m_CurrentX;

        /// <summary>
        /// The Y position in 3D space where we are focusing the camera.
        /// </summary>
        private double m_CurrentY;

        /// <summary>
        /// The Z position in 3D space where we are focusing the camera.
        /// </summary>
        private double m_CurrentZ;

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
        /// Translates a point in the 3D isometric world to a 2D point
        /// on the screen.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="z">The Z position.</param>
        /// <returns>The position on the screen.</returns>
        public Vector2 TranslatePoint(float x, float y, float z)
        {
            float cx = this.Chunk.X;
            float cy = this.Chunk.Y;
            float cz = this.Chunk.Z;
            float xx = this.ChunkCenterX;
            float yy = this.ChunkCenterY;
            xx += (x - cx);
            yy += (x - cx) * 0.75f;
            xx -= (y - cy);
            yy += (y - cy) * 0.75f;
            yy -= (z - cz) / 2f;
            return new Vector2(xx, yy);
        }

        /// <summary>
        /// Checks the current relative position, applying the relative X, Y and Z positions
        /// to it and adjusting the center chunk as needed.
        /// </summary>
        public void Pan(double x, double y, double z)
        {
            // Skip if there is no octree.
            if (this.ChunkOctree == null)
                return;

            // Adjust the position.
            this.m_CurrentX += x;
            this.m_CurrentY += y;
            this.m_CurrentZ += z;

            // Pan current chunk.
            var newChunk = this.ChunkOctree.Get(
                (long) this.m_CurrentX,
                (long) this.m_CurrentY,
                (long) this.m_CurrentZ);
            if (newChunk != null)
                this.Chunk = newChunk;
        }

        /// <summary>
        /// Sets the current focus of the screen such the center of the screen
        /// is focused on the 3D point in the isometric world.
        /// </summary>
        public void Focus(double x, double y, double z)
        {
            this.Pan(x - this.m_CurrentX, y - this.m_CurrentY, z - this.m_CurrentZ);
        }

        public void InitializeRenderContext(IRenderContext renderContext)
        {
            renderContext.View = Matrix.CreateLookAt(
                this.CurrentFocus + new Vector3(15, 30, 15) * 35,
                this.CurrentFocus,
                Vector3.Up);
            renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, 1.0f, 5000.0f);
        }
    }
}