using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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
        protected int GetRandomRange(long x, long y, long z, int end, long modifier = 0)
        {
            unchecked
            {
                int a = this.GetRandomInt(x, y, z, modifier);
                if (a < 0) a += int.MaxValue;
                return a % end;
            }
        }

        /// <summary>
        /// Returns a random positive integer between the specified inclusive start
        /// value and the exclusive end value.
        /// </summary>
        protected int GetRandomRange(long x, long y, long z, int start, int end, long modifier)
        {
            unchecked
            {
                int a = this.GetRandomInt(x, y, z, modifier);
                if (a < 0) a += int.MaxValue;
                return a % (end - start) + start;
            }
        }

        /// <summary>
        /// Returns a random integer over the range of valid integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        protected int GetRandomInt(long x, long y, long z, long modifier = 0)
        {
            unchecked
            {
                return (int)(this.GetRandomNumber(x, y, z, modifier) % int.MaxValue);
            }
        }

        /// <summary>
        /// Returns a random long integer over the range of valid long integers based
        /// on the provided X and Y position, and the specified modifier.
        /// </summary>
        protected long GetRandomLong(long x, long y, long z, long modifier = 0)
        {
            return this.GetRandomNumber(x, y, z, modifier);
        }

        /// <summary>
        /// Returns a random double between the range of 0.0 and 1.0 based on
        /// the provided X and Y position, and the specified modifier.
        /// </summary>
        protected double GetRandomDouble(long x, long y, long z, long modifier = 0)
        {
            long a = this.GetRandomNumber(x, y, z, modifier) / 2;
            if (a < 0) a += long.MaxValue;
            return (double)a / (double)long.MaxValue;
        }

        private long GetRandomNumber(long x, long y, long z, long modifier)
        {
            /* From: http://stackoverflow.com/questions/2890040/implementing-gethashcode
             * Although we aren't implementing GetHashCode, it's still a good way to generate
             * a unique number given a limited set of fields */
            unchecked
            {
                long seed = (x - 1) * 3661988493967 + (y - 1);
                seed += (x - 2) * 2990430311017;
                seed *= (y - 3) * 14475080218213;
                seed += modifier;
                seed += (y - 4) * 28124722524383;
                seed += (z - 5) * 25905201761893;
                seed *= (x - 6) * 16099760261113;
                seed += (x - 7) * this.m_Seed;
                seed *= (y - 8) * this.m_Seed;
                seed += (z - 9) * 55497960863;
                seed *= (z - 10) * 611286883423;
                seed += modifier;
                // Prevents the seed from being 0 along an axis.
                seed += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;

                if (this.m_RandomNumberIndexCache.Keys.Contains(seed))
                    this.m_RandomNumberIndexCache[seed] += 1;
                else
                    this.m_RandomNumberIndexCache[seed] = 0;
                int index = 0;
                try
                {
                    index = this.m_RandomNumberIndexCache[seed];
                }
                catch (Exception) { }

                long rng = seed * seed;
                rng += (x - 11) * 2990430311017;
                rng *= (y - 12) * 14475080218213;
                rng *= (z - 13) * 23281823741513;
                rng += index;
                rng -= seed * 28124722524383;
                rng *= (x - 14) * 16099760261113;
                rng += seed * this.m_Seed;
                rng *= (y - 15) * this.m_Seed;
                rng *= (z - 16) * 18193477834921;
                rng += (x - 199) * (y - 241) * (z - 1471) * 9018110272013;
                rng += modifier;
                rng += 3661988493967;

                return rng;
            }
        }

        public int[] GenerateData(long x, long y, long width, long height)
        {
            if (this.m_RandomNumberIndexCache == null)
                this.m_RandomNumberIndexCache = new Dictionary<long, int>();
            else
                this.m_RandomNumberIndexCache.Clear();
            return this.GenerateDataImpl(x, y, width, height);
        }

        public int[] GenerateData(long x, long y, long z, long width, long height, long depth)
        {
            if (this.m_RandomNumberIndexCache == null)
                this.m_RandomNumberIndexCache = new Dictionary<long, int>();
            else
                this.m_RandomNumberIndexCache.Clear();
            return this.GenerateDataImpl(x, y, z, width, height, depth);
        }

        protected abstract int[] GenerateDataImpl(long x, long y, long width, long height);

        protected virtual int[] GenerateDataImpl(long x, long y, long z, long width, long height, long depth)
        {
            // FIXME: If the depth != 1, then this is an invalid result.
            return this.GenerateData(x, y, width, height);
        }

        public virtual bool IsLayerColorsFlags()
        {
            return false;
        }

        public abstract Dictionary<int, LayerColor> GetLayerColors();

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
