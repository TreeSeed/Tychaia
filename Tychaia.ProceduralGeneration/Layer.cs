﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace Tychaia.ProceduralGeneration
{
    [DataContract]
    public abstract class Layer
    {
        [DataMember]
        private long m_Seed;

        [DataMember]
        private Layer[] m_Parents;

        // Runtime only.
        private Dictionary<long, int> m_RandomNumberIndexCache = new Dictionary<long, int>();

        public Layer[] Parents
        {
            get
            {
                return this.m_Parents;
            }
        }

        public abstract bool Is3D
        {
            get;
        }

        private void TransformSeed()
        {
            // Give this layer's seed some variance from the old parent, since otherwise
            // lower layers will be repeating the same values as the parent layer for cells.
            unchecked
            {
                this.m_Seed *= 2990430311017;
                this.m_Seed += 16099760261113;
            }
        }

        public void SetValues(Layer parent, int seed)
        {
            if (parent == null)
                this.m_Seed = seed;
            else
            {
                this.m_Parents = new Layer[1] { parent };
                this.m_Seed = this.m_Parents[0].m_Seed;
                this.TransformSeed();
            }
        }

        /// <summary>
        /// The world seed.
        /// </summary>
        protected long Seed
        {
            get
            {
                return this.m_Seed;
            }
        }

        protected Layer(long seed)
        {
            this.m_Parents = new Layer[] { };
            this.m_Seed = seed;
        }

        protected Layer(Layer parent)
        {
            this.m_Parents = new Layer[] { parent };
            if (parent != null)
            {
                this.m_Seed = parent.m_Seed;
                this.TransformSeed();
            }
        }

        protected Layer(Layer[] parents)
        {
            if (parents == null)
                throw new ArgumentNullException("parents");
            if (parents.Length < 1)
                throw new ArgumentOutOfRangeException("parents");
            this.m_Parents = parents;
            if (parents[0] != null)
            {
                this.m_Seed = parents[0].m_Seed;
                this.TransformSeed();
            }
        }

        /// <summary>
        /// Returns a random positive integer between the specified 0 and
        /// the exclusive end value.
        /// </summary>
        protected int GetRandomRange(int x, int y, int end, int modifier = 0)
        {
            unchecked
            {
                int a = this.GetRandomInt(x, y, modifier);
                if (a < 0) a += int.MaxValue;
                return a % end;
            }
        }

        /// <summary>
        /// Returns a random positive integer between the specified inclusive start
        /// value and the exclusive end value.
        /// </summary>
        protected int GetRandomRange(int x, int y, int start, int end, int modifier)
        {
            unchecked
            {
                int a = this.GetRandomInt(x, y, modifier);
                if (a < 0) a += int.MaxValue;
                return a % (end - start) + start;
            }
        }

        /// <summary>
        /// Returns a random integer over the range of valid integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        protected int GetRandomInt(int x, int y, int modifier = 0)
        {
            unchecked
            {
                return (int)this.GetRandomNumber(x, y, modifier);
            }
        }

        /// <summary>
        /// Returns a random long integer over the range of valid long integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        protected long GetRandomLong(int x, int y, int modifier = 0)
        {
            return this.GetRandomNumber(x, y, modifier);
        }

        /// <summary>
        /// Returns a random double between the range of 0.0 and 1.0 based on
        /// the provided X and Y position, and the specified modifier.
        /// </summary>
        protected double GetRandomDouble(int x, int y, int modifier = 0)
        {
            return this.GetRandomNumber(x, y, modifier) / (double)long.MaxValue;
        }

        private long GetRandomNumber(int x, int y, int modifier)
        {
            /* From: http://stackoverflow.com/questions/2890040/implementing-gethashcode
             * Although we aren't implementing GetHashCode, it's still a good way to generate
             * a unique number given a limited set of fields */
            unchecked
            {
                long seed = x * 3661988493967 + y;
                seed += x * 2990430311017;
                seed *= y * 14475080218213;
                seed += modifier;
                seed -= y * 28124722524383;
                seed *= x * 16099760261113;
                seed += x * this.m_Seed;
                seed *= y * this.m_Seed;
                seed += modifier;
                // Prevents the seed from being 0 along an axis.
                seed += (x - 199) * (y - 241) * 9018110272013;

                if (this.m_RandomNumberIndexCache.Keys.Contains(seed))
                    this.m_RandomNumberIndexCache[seed] += 1;
                else
                    this.m_RandomNumberIndexCache[seed] = 0;
                int index = this.m_RandomNumberIndexCache[seed];

                long rng = seed * seed;
                rng += x * 2990430311017;
                rng *= y * 14475080218213;
                rng += index;
                rng -= seed * 28124722524383;
                rng *= x * 16099760261113;
                rng += seed * this.m_Seed;
                rng *= y * this.m_Seed;
                rng += modifier;

                return rng;
            }
        }

        public int[] GenerateData(int x, int y, int width, int height)
        {
            if (this.m_RandomNumberIndexCache == null)
                this.m_RandomNumberIndexCache = new Dictionary<long, int>();
            else
                this.m_RandomNumberIndexCache.Clear();
            return this.GenerateDataImpl(x, y, width, height);
        }

        public int[] GenerateData(int x, int y, int z, int width, int height, int depth)
        {
            if (this.m_RandomNumberIndexCache == null)
                this.m_RandomNumberIndexCache = new Dictionary<long, int>();
            else
                this.m_RandomNumberIndexCache.Clear();
            return this.GenerateDataImpl(x, y, z, width, height, depth);
        }

        protected abstract int[] GenerateDataImpl(int x, int y, int width, int height);

        protected virtual int[] GenerateDataImpl(int x, int y, int z, int width, int height, int depth)
        {
            // FIXME: If the depth != 1, then this is an invalid result.
            return this.GenerateData(x, y, width, height);
        }

        public abstract Dictionary<int, System.Drawing.Brush> GetLayerColors();

        #region Designer Methods

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual string[] GetParentsRequired()
        {
            return new string[] { "Parent" };
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public abstract bool[] GetParents3DRequired();

        public void SetParent(int idx, Layer parent)
        {
            if (this.m_Parents.Length <= idx)
            {
                // Resize up.
                Layer[] newParents = new Layer[idx + 1];
                for (int i = 0; i < this.m_Parents.Length; i++)
                    newParents[i] = this.m_Parents[i];
                newParents[idx] = parent;
                this.m_Parents = newParents;
            }
            else
                this.m_Parents[idx] = parent;

            // Check to ensure the parent is valid.
            bool is3D = this.GetParents3DRequired()[idx];
            if (is3D && parent is Layer2D)
                this.m_Parents[idx] = null;
            else if (!is3D && parent is Layer3D)
                this.m_Parents[idx] = null;

            // Adjust seed if needed.
            if (idx == 0)
            {
                this.m_Seed = this.m_Parents[idx].m_Seed;
                this.TransformSeed();
            }
        }

        #endregion
    }
}
