// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Tychaia.Globals;

namespace Tychaia
{
    /// <summary>
    /// Maintains a hash tablet of resulting renders for input data.  If the input data
    /// for two chunks is the same, there is no need to store two seperate render targets
    /// in graphics RAM; we can just use the same textures.
    /// </summary>
    public static class UniqueRenderCache
    {
        private static Dictionary<long, UniqueRenderPair> m_RenderPairs = new Dictionary<long, UniqueRenderPair>();

        public static bool Has(int[] data)
        {
            return m_RenderPairs.Keys.Contains(GetHash(data));
        }

        public static bool IsWaiting(int[] data)
        {
            long hash = GetHash(data);
            if (m_RenderPairs.Keys.Contains(hash))
                return m_RenderPairs[hash] == null;
            else
                throw new InvalidOperationException();
        }

        public static UniqueRender Store(int[] data, RenderTarget2D target, RenderTarget2D depth)
        {
            long hash = GetHash(data);
            UniqueRenderPair urp;
            if (m_RenderPairs.Keys.Contains(hash) && m_RenderPairs[hash] == null /* is waiting */)
            {
                // Remove before new store.
                FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing waiting result.");
                m_RenderPairs.Remove(hash);
            }
            if (m_RenderPairs.Keys.Contains(hash))
            {
                // This is a copy of an existing render, free the passed parameters.
                target.Dispose();
                depth.Dispose();
                urp = m_RenderPairs[hash];
                urp.ReferenceCount += 1;
                FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Storing existing render result (ref+1).");
            }
            else
            {
                urp = new UniqueRenderPair();
                urp.ReferenceCount = 1;
                urp.Target = target;
                urp.DepthMap = depth;
                m_RenderPairs.Add(hash, urp);
                FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Storing new render result (ref=1,mem+1).");
            }
            return new UniqueRender(urp.Target, urp.DepthMap);
        }

        public static void StoreWaiting(int[] data)
        {
            long hash = GetHash(data);
            if (m_RenderPairs.Keys.Contains(hash))
                return;
            FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Storing waiting render result.");
            m_RenderPairs.Add(hash, null);
        }

        public static UniqueRender Grab(int[] data)
        {
            long hash = GetHash(data);
            if (!m_RenderPairs.Keys.Contains(hash))
                throw new InvalidOperationException();
            UniqueRenderPair urp = m_RenderPairs[hash];
            if (urp == null)
                throw new InvalidOperationException();
            urp.ReferenceCount += 1;
            FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Grabbing existing render result (ref+1).");
            return new UniqueRender(urp.Target, urp.DepthMap);
        }

        public static void Release(int[] data)
        {
            long hash = GetHash(data);
            if (!m_RenderPairs.Keys.Contains(hash))
                throw new InvalidOperationException();
            UniqueRenderPair urp = m_RenderPairs[hash];
            urp.ReferenceCount -= 1;
            if (urp.ReferenceCount == 0)
            {
                urp.Target.Dispose();
                urp.DepthMap.Dispose();
                m_RenderPairs.Remove(hash);
                FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing and freeing render result (ref-1,mem-1).");
            }
            else
                FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing render result (ref-1).");
        }

        public static void ReleaseWaiting(int[] data)
        {
            long hash = GetHash(data);
            if (!m_RenderPairs.Keys.Contains(hash))
                throw new InvalidOperationException();
            UniqueRenderPair urp = m_RenderPairs[hash];
            if (urp != null)
                return;
            m_RenderPairs.Remove(hash);
            FilteredConsole.WriteLine(FilterCategory.UniqueRendering, "Releasing waiting result.");
        }

        private static long GetHash(int[] data)
        {
            unchecked
            {
                long hash = 5610979583159;
                for (int i = 0; i < data.Length; i++)
                {
                    hash = hash ^ data[i] ^ i * 6187629050149;
                    hash += 16138338218447;
                    hash = hash ^ data[i] ^ i * 6187629050149;
                    hash += 16138338218447;
                    hash = hash ^ data[i] ^ i * 6187629050149;
                    hash += 16138338218447;
                }
                return hash;
            }
        }

        private class UniqueRenderPair
        {
            public uint ReferenceCount;
            public RenderTarget2D Target;
            public RenderTarget2D DepthMap;
        }

        public class UniqueRender
        {
            public RenderTarget2D Target;
            public RenderTarget2D DepthMap;

            public UniqueRender(RenderTarget2D target, RenderTarget2D depthMap)
            {
                this.Target = target;
                this.DepthMap = depthMap;
                target.Disposing += (sender, e) => { this.Target = null; };
                depthMap.Disposing += (sender, e) => { this.DepthMap = null; };
            }
        }
    }
}
